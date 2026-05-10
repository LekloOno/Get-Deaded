using Godot;

[GlobalClass]
public partial class GCD_Armor : GCD_Health
{
    [Export(PropertyHint.Range, "0.0,1.0")]
    private float _resistance;
    [Export(PropertyHint.Range, "0.0,500.0")]
    private float _maxReduction;

    public override GC_Health BuildNode() =>
        new GC_Armor(MaxHealth, Child?.BuildNode(), _resistance, _maxReduction);
}