using System;
using Godot;

[GlobalClass]
public partial class GC_Armor : GC_Health
{
    [Export(PropertyHint.Range, "0.0,1.0")]
    private float _resistance;
    [Export(PropertyHint.Range, "0.0,500.0")]
    private float _maxReduction;
    public GC_Armor(float maxHealth) : base(maxHealth) {}
    public GC_Armor(float maxHealth, GC_Health child) : base(maxHealth, child) {}
    public GC_Armor(float maxHealth, float initialHealth, GC_Health child) : base(maxHealth, initialHealth, child) {}

    protected override float ModifiedDamage(float damage) => ConstantReduction(damage, _resistance, _maxReduction);

    private static float ConstantReduction(float damage, float resistance, float maxReduction)
    {
        float reduction = Mathf.Min(damage * resistance, maxReduction);
        return damage - reduction;
    }
}