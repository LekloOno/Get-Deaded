using Godot;

[GlobalClass]
public partial class GC_Armor : GC_Health
{
    [Export(PropertyHint.Range, "0.0,1.0")]
    private float _resistance;
    [Export(PropertyHint.Range, "0.0,500.0")]
    private float _maxReduction;

    protected override float ReductionFromDamage(float damage) => Mathf.Min(damage * _resistance, _maxReduction);
    protected override float DamageFromReduction(float reduction)
    {
        /*
        if (reduction == _maxReduction)
            return _maxReduction / _resistance;

        The strict method should include this, but reduction should never be higher than maxReduction anyway
        */
        return reduction / _resistance;
    }
}