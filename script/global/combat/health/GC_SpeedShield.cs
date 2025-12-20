using Godot;

[GlobalClass]
public partial class GC_SpeedShield : GC_Health
{
    [Export] private GM_Mover _mover;
    [Export] private float _dps;
    [Export] private Curve _healCurve;
    public bool Active
    {
        get => IsPhysicsProcessing();
        set => SetPhysicsProcess(value);
    }

    public override void _Ready()
    {
        Active = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        float diff = (ComputeHeal() - _dps) * (float) delta;

        if (diff > 0)
            Regen(diff);
        else
            base.TakeDamage(diff*-1f, out float _, out float __, out GC_Health ___);
    }

    private float ComputeHeal()
    {
        float speed = MATH_Vector3Ext.Flat(_mover.Velocity).Length();
        return _healCurve.Sample(speed);
    }

    public void Regen(float amount)
    {
        float realHeal = Mathf.Min(amount, _maxHealth - CurrentHealth);
        CurrentHealth += realHeal;

        OnHeal?.Invoke(this, DamageArgs(realHeal));
    }
}