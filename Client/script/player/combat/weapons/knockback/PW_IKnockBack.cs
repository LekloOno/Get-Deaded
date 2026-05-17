using Godot;

/// <summary>
/// Base abstraction for a PW_Shot KnockBack handler.
/// </summary>
public interface PW_IKnockBack
{
    /// <summary>
    /// Retrieves the knock knock back nullable impulse for the given direction and hitSize. <br/>
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="hitSize"></param>
    /// <returns></returns>
    Vector3? KnockBackImpulse(Vector3 direction, float hitSize = 1f);
    
    MATH_AdditiveModifiers KnockBackMultiplier {get;}
    MATH_FlatVec3Modifiers KnockBackDirFlatAdd {get;}
}