using Godot;

[GlobalClass]
public partial class GC_Hitbox : Area3D, GC_IHitDealer
{
    [Export] private GC_Hit _hit;
    [Export] private float _cd;
    private bool _active = true;
    private SceneTreeTimer _reset;

    public GC_Hit HitData => _hit;

    public GE_IActiveCombatEntity OwnerEntity => GE_Environment.Instance;

    public override void _Ready()
    {
        AreaEntered += ProcessCollision;
        _hit.InitializeModifiers();
    }

    private void ProcessCollision(Area3D area3D)
    {
        if (!_active) return;

        if (area3D is GC_HurtBox hurtBox)
        {
            hurtBox.Damage(this, out _, out _, out _, out _, out _);
            _active = false;
            _reset = GetTree().CreateTimer(_cd, false, true);
            _reset.Timeout += Reset;
        }
    }

    public void Reset()
    {
        _active = true;
        if (_reset != null)
        {
            _reset.Timeout -= Reset;
            _reset = null;
        }
    }

    public void Shoot(){}
    public void Interrupt(){}
}