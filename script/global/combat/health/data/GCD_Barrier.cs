using Godot;

[GlobalClass]
public partial class GCD_Barrier : GCD_Health
{
    [Export(PropertyHint.Range, "0.0,1.0")]
    private float _resistance;
    [Export(PropertyHint.Range, "0,1000")]
    private ulong _coolDown;

    protected override GC_Health BuildSelf(GC_Health child) =>
        new GC_Barrier(MaxHealth, child, _resistance, _coolDown);
}