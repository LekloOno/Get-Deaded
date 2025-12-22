using System.Linq;
using Godot;

/// <summary>
///
/// </summary>
public class MATH_FlatVec3Modifiers : MATH_PropertyModifier<Vector3>
{
    public override Vector3 Result() => _modifiers.Aggregate(Vector3.Zero, (acc, v) => acc + v);
}