using Godot;

[GlobalClass]
public partial class GC_HealBox: Area3D
{
    [Export] private GC_Hit _hit;
    [Export] private float _cd;
    private bool _active = true;
    private SceneTreeTimer _reset;

    public override void _Ready()
    {
        AreaEntered += ProcessCollision;
    }

    private void ProcessCollision(Area3D area3D)
    {
        if (!_active) return;

        if (area3D is GC_HurtBox hurtBox)
        {
            hurtBox.Heal(_hit.GetDamage(hurtBox.BodyPart));
            _active = false;
            _reset = GetTree().CreateTimer(_cd);
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
}