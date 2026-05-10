using Godot;

[GlobalClass]
public partial class GCD_Barrier : GCD_Health
{
    [Export(PropertyHint.Range, "0.0,1.0")]
    private float _resistance;
    [Export(PropertyHint.Range, "0,1000")]
    private ulong _coolDown;

    public override GC_Health BuildNode() =>
        new GC_Barrier(MaxHealth, Child?.BuildNode(), _resistance, _coolDown);
}