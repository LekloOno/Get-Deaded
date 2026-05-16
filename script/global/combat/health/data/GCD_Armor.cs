using Godot;

[GlobalClass]
public partial class GCD_Armor : GCD_Health
{
    [Export(PropertyHint.Range, "0.0,1.0")]
    private float _resistance;
    [Export(PropertyHint.Range, "0.0,500.0")]
    private float _maxReduction;

    protected override GC_Health BuildSelf(GC_Health child) =>
        new GC_Armor(MaxHealth, child, _resistance, _maxReduction);
}