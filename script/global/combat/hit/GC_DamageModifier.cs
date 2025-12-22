using Godot;

namespace Pew;

[GlobalClass]
public partial class GC_DamageModifier : Resource
{
    [Export] public GC_BodyPart BodyPart { get; private set;} = GC_BodyPart.Chest;
    [Export] public float Modifier { get; private set;} = 1f;

    public GC_DamageModifier() :  this(GC_BodyPart.Chest, 1f) {}

    public GC_DamageModifier(GC_BodyPart bodyPart, float modifier)
    {
        BodyPart = bodyPart;
        Modifier = modifier;
    }

    public static implicit operator float(GC_DamageModifier damageModifier) => damageModifier.Modifier;
}
