using Godot;

[GlobalClass]
public partial class PW_SimpleADS : PW_ADS
{
    [Export] public float SpreadMultiplier {get; private set;} = 0f;
    [Export] public float RecoilMultiplier {get; private set;} = 0f;
}