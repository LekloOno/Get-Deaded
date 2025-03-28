using Godot;
using Godot.Collections;

public partial class E_Ennemy : CharacterBody3D
{
    [Export] private GC_HealthManager _healthManager;
    [Export] private float _hideDelay;
    [Export] private MeshInstance3D _surfaceMesh;
    [Export] private MeshInstance3D _jointMesh;
    [Export] private GL_Dropper _lootDropper;
    private StandardMaterial3D _surfaceMeshMaterial;
    private StandardMaterial3D _jointMeshMaterial;
    private SceneTreeTimer _hideTimer;
    public HealthEventHandler OnDie { get => _healthManager.TopHealthLayer.OnDie; set => _healthManager.TopHealthLayer.OnDie = value;}

    public override void _Ready()
    {
        if(_surfaceMesh?.Mesh.SurfaceGetMaterial(0) is StandardMaterial3D surfaceMat)
            _surfaceMeshMaterial = surfaceMat;

        if(_jointMesh?.Mesh.SurfaceGetMaterial(0) is StandardMaterial3D jointMat)
            _jointMeshMaterial = jointMat;

        Enable();

        OnDie += PlayDeath;
    }

    public void PlayDeath(GC_Health health)
    {
        _lootDropper.Drop();
        Disable();
    }

    public void Disable()
    {
        CollisionLayer = 0;
        SetPhysicsProcess(false);
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
        
        Velocity = velocity;
        
        MoveAndSlide();
    }
}