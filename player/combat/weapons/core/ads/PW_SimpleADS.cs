using Godot;

public class SimpleADSModifiers(float moveSpeed, float spread, float recoil)
{
    public static readonly SimpleADSModifiers INIT = new(1f, 1f, 1f);
    public float MoveSpeed {get;} = moveSpeed;
    public float Spread {get;} = spread;
    public float Recoil {get;} = recoil;
}

public partial class PW_SimpleADS : PW_ADS<SimpleADSModifiers>
{
    [Export] public float SpreadMultiplier {get; private set;} = 1f;
    [Export] public float RecoilMultiplier {get; private set;} = 1f;

    protected override SimpleADSModifiers GetInitValue() => SimpleADSModifiers.INIT;
    protected override SimpleADSModifiers GetModifiers() => new(MoveSpeedMultiplier, SpreadMultiplier, RecoilMultiplier);
}