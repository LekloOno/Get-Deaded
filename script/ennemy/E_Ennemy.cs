using Godot;
using Godot.Collections;

public partial class E_Ennemy : CharacterBody3D
{
    [Export] private GC_HealthManager _healthManager;
    [Export] private Array<GC_HurtBox> _hurtBoxes;
    [Export] private float _hideDelay;
    private SceneTreeTimer _hideTimer;
    public HealthEventHandler OnDie { get => _healthManager.TopHealthLayer.OnDie; set => _healthManager.TopHealthLayer.OnDie = value;}

    public override void _Ready()
    {
        Enable();
    }

    public void Disable()
    {
        SetPhysicsProcess(false);
        foreach (GC_HurtBox hurtBox in _hurtBoxes)
            hurtBox.CollisionLayer = 0;

        _hideTimer = GetTree().CreateTimer(_hideDelay);
        _hideTimer.Timeout += Hide;
    }

    public void DisablePhysics()
    {
        SetPhysicsProcess(false);
        foreach (GC_HurtBox hurtBox in _hurtBoxes)
            hurtBox.CollisionLayer = 0;
    }

    public void Enable()
    {
        Show();
        SetPhysicsProcess(true);
        foreach (GC_HurtBox hurtBox in _hurtBoxes)
            hurtBox.CollisionLayer = 2;

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