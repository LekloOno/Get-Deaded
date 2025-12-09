using System;
using Godot;

public partial class E_Enemy : GB_CharacterBody, E_IEnemy
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
    [Export] private uint _score = 0;
    [Export] private GC_Health _healthOverride = null;

    private ShaderMaterial _surfaceMeshMaterial;
    private ShaderMaterial _jointMeshMaterial;
    private SceneTreeTimer _hideTimer;
    //public HealthEventHandler OnDie { get => _healthManager.TopHealthLayer.OnDie; set => _healthManager.TopHealthLayer.OnDie = value;}
    //public HealthEventHandler<DamageEventArgs> OnDamage { get => _healthManager.TopHealthLayer.OnDamage; set => _healthManager.TopHealthLayer.OnDamage = value;}
    public EnemyHealthEventHandler OnDie {get; set;}
    public EnemyHealthEventHandler<DamageEventArgs> OnDamage {get; set;}

    private BaseMaterial3D.ShadingModeEnum _initialJointShadingMode;
    private Color _initialColor;
    private Color _initialJointColor;
    private SceneTreeTimer _hitResetTimer;

    public float Alpha
    {
        get => ((Color) _surfaceMeshMaterial.GetShaderParameter("albedo")).A;
        set
        {
            Color color = (Color) _surfaceMeshMaterial.GetShaderParameter("albedo");
            color.A = value;
            _surfaceMeshMaterial.SetShaderParameter("albedo", color);
            Color jointColor = (Color) _jointMeshMaterial.GetShaderParameter("albedo");
            jointColor.A = value;
            _jointMeshMaterial.SetShaderParameter("albedo", jointColor);
        }
    }

    public uint Score => _score;

    public GC_HealthManager HealthManager => _healthManager;

    public PW_WeaponsHandler WeaponsHandler => null;

    public GB_IExternalBodyManager Body => this;

    

    public override void _Ready()
    {
        if (_healthOverride != null)
            _healthManager.TopHealthLayer = _healthOverride;

        _healthManager.TopHealthLayer.OnDie += (layer) => OnDie?.Invoke(this, layer);
        _healthManager.TopHealthLayer.OnDamage += (layer, damageArgs) => OnDamage?.Invoke(this, layer, damageArgs);

        if(_surfaceMesh?.Mesh.SurfaceGetMaterial(0) is ShaderMaterial surfaceMat)
        {
            _surfaceMeshMaterial = surfaceMat;
            _initialColor = (Color) surfaceMat.GetShaderParameter("albedo");
        }

        if(_jointMesh?.Mesh.SurfaceGetMaterial(0) is ShaderMaterial jointMat)
        {
            _jointMeshMaterial = jointMat;
            _initialJointColor = (Color) jointMat.GetShaderParameter("albedo");
        }

        Enable();
        

        OnDie += PlayDeath;
        OnDamage += PlayHit;
    }

    private void PlayHit(E_IEnemy _, GC_Health senderLayer, DamageEventArgs e)
    {
        _surfaceMeshMaterial.SetShaderParameter("albedo", _hitColor);

        if (_hitResetTimer != null)
            _hitResetTimer.Timeout -= ResetHitMaterial;

        _hitResetTimer = GetTree().CreateTimer(_hitTime);
        _hitResetTimer.Timeout += ResetHitMaterial;
    }

    private void ResetHitMaterial()
    {
        _surfaceMeshMaterial.SetShaderParameter("albedo", _initialColor);
        _jointMeshMaterial.SetShaderParameter("albedo", _initialJointColor);
    }

    public void PlayDeath(E_IEnemy _, GC_Health health)
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
        opacityTween.TweenProperty(this, "Alpha", 0f, _hideDelay);

        await ToSignal(opacityTween, "finished");
        
        Hide();
        
        SetPhysicsProcess(false);
    }

    public void Enable()
    {
        CollisionLayer = CONF_Collision.Layers.EnvironmentEntity;
        Tween surfaceTween = CreateTween();
        surfaceTween.TweenProperty(this, "Alpha", 1f, 0.2f);

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