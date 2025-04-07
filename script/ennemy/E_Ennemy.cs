using System;
using Godot;

public partial class E_Ennemy : CharacterBody3D
{
    [Export] private GC_HealthManager _healthManager;
    [Export] private float _hideDelay;
    [Export] private MeshInstance3D _surfaceMesh;
    [Export] private MeshInstance3D _jointMesh;
    [Export] private GL_Dropper _lootDropper;
    [Export] private PCT_Undirect _traumaCauser;
    [Export] private float _drag = 10f;
    [Export] private Color _hitColor = new(1f, 1f, 1f, 1f);
    [Export] private float _hitTime = 0.15f;
    private StandardMaterial3D _surfaceMeshMaterial;
    private StandardMaterial3D _jointMeshMaterial;
    private SceneTreeTimer _hideTimer;
    public HealthEventHandler OnDie { get => _healthManager.TopHealthLayer.OnDie; set => _healthManager.TopHealthLayer.OnDie = value;}
    public HealthEventHandler<DamageEventArgs> OnDamage { get => _healthManager.TopHealthLayer.OnDamage; set => _healthManager.TopHealthLayer.OnDamage = value;}

    private BaseMaterial3D.ShadingModeEnum _initialShadingMode;
    private BaseMaterial3D.ShadingModeEnum _initialJointShadingMode;
    private Color _initialColor;
    private Color _initialJointColor;
    private SceneTreeTimer _hitResetTimer;

    public override void _Ready()
    {
        if(_surfaceMesh?.Mesh.SurfaceGetMaterial(0) is StandardMaterial3D surfaceMat)
        {
            _surfaceMeshMaterial = surfaceMat;
            _initialColor = surfaceMat.AlbedoColor;
            _initialShadingMode = surfaceMat.ShadingMode;
        }

        if(_jointMesh?.Mesh.SurfaceGetMaterial(0) is StandardMaterial3D jointMat)
        {
            _jointMeshMaterial = jointMat;
            _initialJointColor = jointMat.AlbedoColor;
            _initialJointShadingMode = jointMat.ShadingMode;
        }

        Enable();
        

        OnDie += PlayDeath;
        OnDamage += PlayHit;
    }

    private void PlayHit(GC_Health senderLayer, DamageEventArgs e)
    {
        _surfaceMeshMaterial.AlbedoColor = _jointMeshMaterial.AlbedoColor = _hitColor;
        _surfaceMeshMaterial.ShadingMode = _jointMeshMaterial.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;

        if (_hitResetTimer != null)
            _hitResetTimer.Timeout -= ResetHitMaterial;

        _hitResetTimer = GetTree().CreateTimer(_hitTime);
        _hitResetTimer.Timeout += ResetHitMaterial;
    }

    private void ResetHitMaterial()
    {
        _surfaceMeshMaterial.AlbedoColor = _initialColor;
        _surfaceMeshMaterial.ShadingMode = _initialShadingMode;
        _jointMeshMaterial.AlbedoColor = _initialJointColor;
        _jointMeshMaterial.ShadingMode = _initialJointShadingMode;
    }

    public void PlayDeath(GC_Health health)
    {
        _lootDropper.Drop();
        _traumaCauser.CauseTrauma();
        Disable();
    }

    public void Disable()
    {
        CollisionLayer = 0;
        _healthManager.DisableHurt();

        //_hideTimer = GetTree().CreateTimer(_hideDelay);
        //_hideTimer.Timeout += Hide;
        
        HideMesh();
    }

    public async void HideMesh()
    {
        Tween opacityTween = CreateTween();
        opacityTween.TweenProperty(_surfaceMeshMaterial, "albedo_color:a", 0f, _hideDelay);
        Tween jointTween = CreateTween();
        jointTween.TweenProperty(_jointMeshMaterial, "albedo_color:a", 0f, _hideDelay);

        await ToSignal(opacityTween, "finished");
        
        Hide();
        
        SetPhysicsProcess(false);
    }

    public void Enable()
    {
        CollisionLayer = CONF_Collision.Layers.EnvironmentEntity;
        Tween surfaceTween = CreateTween();
        surfaceTween.TweenProperty(_surfaceMeshMaterial, "albedo_color:a", 1f, 0.2f);
        Tween jointTween = CreateTween();
        jointTween.TweenProperty(_jointMeshMaterial, "albedo_color:a", 1f, 0.2f);

        Show();
        SetPhysicsProcess(true);
        _healthManager.EnableHurt(CONF_Collision.Layers.EnnemiesHurtBox);
    
        _healthManager.Init(true);
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = Velocity;
        if (!IsOnFloor())
            velocity += GetGravity() * (float) delta;
        else
            velocity -= velocity * (float) delta * _drag;
        
        Velocity = velocity;
        
        MoveAndSlide();
    }
}