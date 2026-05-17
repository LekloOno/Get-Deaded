using Godot;

[GlobalClass]
public partial class PWK_Simple : PW_KnockBack
{
    [Export] protected float _knockBack = 0f;
    /// <summary>
    /// Retrieves the strength of the knockBack impulse for a given hitSize. <br/>
    /// It can be freely overwritten by implementing classes.
    /// </summary>
    /// <param name="hitSize"></param>
    /// <returns></returns>
    protected virtual float KnockBackStrength(float hitSize) =>
        _knockBack * hitSize
        * KnockBackMultiplier.Result();

    /// <summary>
    /// Retrieves the knock knock back impulse for the given direction and hitSize. <br/>
    /// It can be freely overwritten by implementing classes.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="hitSize"></param>
    /// <returns></returns>
    protected virtual Vector3 BaseKnockBackImpulse(Vector3 direction, float hitSize)
    {
        float strength = KnockBackStrength(hitSize);

        Vector3 baseImpulse = strength * direction.Normalized();
        return baseImpulse + KnockBackDirFlatAdd.Result();
    }

    public override Vector3? KnockBackImpulse(Vector3 direction, float hitSize)
    {
        if (_knockBack == 0f || hitSize == 0f)
            return null;

        Vector3 impulse = BaseKnockBackImpulse(direction, hitSize);
        if (impulse.Length() == 0f)
            return null;

        return impulse;
    }
}