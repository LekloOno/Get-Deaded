using System;
using Godot;

public partial class E_Enemy : GB_CharacterBody, E_IEnemy
{
    [Export] private GC_HealthManager _healthManager;
    [Export] private float _hideDelay;
    [Export] private MeshInstance3D _surfaceMesh;
    [Export] private MeshInstance3D _jointMesh;
    [Export] private GL_Dropper _lootDropper;
    [Export] private float _drag = 10f;
    [Export] private Color _hitColor = new(1f, 1f, 1f, 1f);
    [Export] private float _hitTime = 0.15f;
    [Export] private uint _score = 0;
    [Export] private GC_Health _healthOverride = null;
    [Export] private PhysicalBoneSimulator3D _ragdolSimulator;
    [Export] private Skeleton3D _skeleton;
    [Export] private PROTO_Mover _mover;
    [Export] private bool _aim;
    [Export] public PCT_SimpleTraumaData KillTraumaData {get; private set;}
    [Export] private PW_FireBis _fire;
    [Export] private float _speedSpreadFactor = 10f;
    [Export] private double _reactionTime = 0.2f;
    private double _timeOnSight = 0f;
    private bool _shooting = false;


    public bool Enabled {get; private set;} = false;

    private ShaderMaterial _surfaceMeshMaterial;
    private ShaderMaterial _jointMeshMaterial;
    private SceneTreeTimer _hideTimer;
    //public HealthEventHandler OnDie { get => _healthManager.TopHealthLayer.OnDie; set => _healthManager.TopHealthLayer.OnDie = value;}
    //public HealthEventHandler<DamageEventArgs> OnDamage { get => _healthManager.TopHealthLayer.OnDamage; set => _healthManager.TopHealthLayer.OnDamage = value;}
    public EnemyHealthEventHandler OnDie {get; set;}
    public EnemyDisableEventHandler OnDisable {get; set;}
    public EnemyHealthEventHandler<DamageEventArgs> OnDamage {get; set;}

    private BaseMaterial3D.ShadingModeEnum _initialJointShadingMode;
    private Color _initialColor;
    private Color _initialJointColor;
    private SceneTreeTimer _hitResetTimer;
    private GE_ICombatEntity _target;
    private Node3D _targetNode;

    public void SetTarget(Node3D target) => _mover.Target = target;

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

    public GE_ICombatEntity Target
    {
        get => _target;
        set {
            if (value is Node3D node)
                _mover.Target = node;
            _target = value;
        }
    }


    public override void _Ready()
    {
        SetProcess(false);
        _fire?.Initialize(this);

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

        Spawn();

        OnDie += PlayDeath;
        OnDamage += PlayHit;
    }

    private void PlayHit(E_IEnemy _, GC_Health senderLayer, DamageEventArgs e)
    {
        _surfaceMeshMaterial.SetShaderParameter("albedo", _hitColor);

        if (_hitResetTimer != null)
            _hitResetTimer.Timeout -= ResetHitMaterial;

        _hitResetTimer = GetTree().CreateTimer(_hitTime, false, true);
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
        _ragdolSimulator?.PhysicalBonesStartSimulation();
        Pool();
    }

    public void Pool()
    {
        _fire?.Disable();
        _shooting = false;

        if (!Enabled)
            return;

        SetProcess(false);
        
        Velocity = Vector3.Zero;

        Enabled = false;
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
        OnDisable?.Invoke(this);
    }

    public void Spawn()
    {
        if (Enabled)
            return;

        _fire?.Enable();

        if (_mover != null)
            SetProcess(true);

        Enabled = true;
        CollisionLayer = CONF_Collision.Layers.EnvironmentEntity;
        Tween surfaceTween = CreateTween();
        surfaceTween.TweenProperty(this, "Alpha", 1f, 0.2f);

        Show();
        SetPhysicsProcess(true);
        _healthManager.EnableHurt(CONF_Collision.Layers.EnnemiesHurtBox);
        _ragdolSimulator?.PhysicalBonesStopSimulation();
        _skeleton?.ResetBonePoses();
    
        _healthManager.Init(true);
    }

    protected override void PhysicsProcessSpec(double delta)
    {
        if (!Enabled)
            return;

        Vector3 velocity = Velocity;
        if (!IsOnFloor())
            velocity += GetGravity() * (float) delta;
        else
        {
            velocity = ApplyDrag(velocity, delta);
            
            if (_mover != null)
                velocity += _mover.GetAcceleration(velocity, delta);
        }
        
        Velocity = velocity;
        
        MoveAndSlide();

        Attack(delta);
    }

    public Vector3 ApplyDrag(Vector3 velocity, double deltaTime)
    {
        float dragFactor = 1f/(1f+(float)deltaTime*_drag);    // Transform the drag to a velocity coeficient
        return velocity * dragFactor;
    }

    public override void _Process(double delta)
    {
        if (_aim)
            _mover.Rotate(this);
    }

    public void Attack(double delta)
    {
        if (_fire == null || _target == null)
            return;

        Vector3 from = _fire.GlobalPosition;
        Vector3 to = _target.Body.GlobalTransform.Origin;

        var spaceState = GetWorld3D().DirectSpaceState;

        var query = PhysicsRayQueryParameters3D.Create(from, to);

        query.CollisionMask = 1;
        var result = spaceState.IntersectRay(query);
        
        if (result.Count == 0)
            _timeOnSight += delta;
        else
            _timeOnSight = 0;

        bool nextShoot = _timeOnSight >= _reactionTime;

        if (nextShoot)
            Aim();

        if (_shooting == nextShoot)
            return;

        if (nextShoot)
            _shooting = _fire.Press();
        else
            _shooting = !_fire.Release();
    }

    private void Aim()
    {
        float spread = SpreadFromTarget() * _speedSpreadFactor;
        LookAtWithSpread(_fire, _target.Body.GlobalTransform.Origin, spread);
    }

    public float SpreadFromTarget()
    {
        Vector3 delta = GlobalPosition - _target.Body.GlobalTransform.Origin;
        Vector3 direction = delta.Normalized();
        Vector3 targetVel = _target.Body.Velocity();

        float lateralSpeed = (targetVel - targetVel.Dot(direction) * direction).Length();
        return lateralSpeed / delta.Length();
    }

    public static void LookAtWithSpread(
        Node3D node,
        Vector3 targetPosition,
        float spreadDegrees,
        Vector3? up = null)
    {
        Vector3 upVector = up ?? Vector3.Up;
        Vector3 direction = (targetPosition - node.GlobalPosition).Normalized();
        Vector3 spreadDirection = ApplySpread(direction, spreadDegrees);
        Vector3 lookPoint = node.GlobalPosition + spreadDirection;

        node.LookAt(lookPoint, upVector);
    }

    private static Vector3 ApplySpread(Vector3 direction, float spreadDegrees)
    {
        if (spreadDegrees <= 0f)
            return direction;

        var rng = new RandomNumberGenerator();

        Vector3 randomAxis = direction.Cross(
            new Vector3(
                rng.RandfRange(-1f, 1f),
                rng.RandfRange(-1f, 1f),
                rng.RandfRange(-1f, 1f)
            ).Normalized()
        ).Normalized();

        float angle = Mathf.DegToRad(
            rng.RandfRange(-spreadDegrees, spreadDegrees)
        );

        return direction.Rotated(randomAxis, angle).Normalized();
    }
}