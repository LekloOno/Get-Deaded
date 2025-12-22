using System.Linq;

/// <summary>
/// Store some run-time modulable modifiers as percent modifiers to use on a float property.
/// <para> The resulting percent modifier is the addition of all currently stored modifiers. </para>
/// <para> It is the modifier disposer's responsibility to remove them when it should. </para>
/// </summary>
public class MATH_AdditiveModifiers : MATH_PropertyModifier<float>
{
    public override float Result() => _modifiers.Aggregate(1f, (acc, v) => acc + v);
}