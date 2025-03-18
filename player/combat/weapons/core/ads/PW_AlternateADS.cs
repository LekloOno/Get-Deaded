using Godot;

[GlobalClass]
public partial class PW_AlternateADS : PW_ADS<PW_Fire>
{
    private PW_Fire _primaryFire;
    private PW_Fire _secondaryFire;

    public void AlternateInitialize(PW_Fire primaryFire, PW_Fire secondaryFire)
    {
        _primaryFire = primaryFire;
        _secondaryFire = secondaryFire;
    }

    protected override PW_Fire GetInitValue() => _primaryFire;
    protected override PW_Fire GetModifiers() => _secondaryFire;
}