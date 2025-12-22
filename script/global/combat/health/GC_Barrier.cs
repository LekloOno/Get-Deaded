using Godot;

namespace Pew;

[GlobalClass]
public partial class GC_Barrier : GC_Health
{
    [Export(PropertyHint.Range, "0.0,1.0")]
    private float _resistance;
    [Export(PropertyHint.Range, "0,1000")]
    private ulong _coolDown;
    private ulong _lastReduction = 0;

    private bool CanReduce() => PHX_Time.ScaledTicksMsec - _lastReduction >= _coolDown;
    private bool JustReduced() => PHX_Time.ScaledTicksMsec == _lastReduction;

    protected override float ReductionFromDamage(float damage)
    {
        if (!CanReduce())
            return 0f;
        
        _lastReduction = PHX_Time.ScaledTicksMsec;
        return damage * _resistance; 
    }

    protected override float HandledDamage(float damage)
    {
        if (!JustReduced())
            return CurrentHealth;
        
        return CurrentHealth / (1f-_resistance);
    }

    protected override float DamageFromReduction(float reduction) => reduction / _resistance;
}