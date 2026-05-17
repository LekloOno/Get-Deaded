using Godot;

[GlobalClass]
public abstract partial class PW_KnockBack : WeaponComponent, PW_IKnockBack
{
    public MATH_AdditiveModifiers KnockBackMultiplier {get; protected set;} = new();
    public MATH_FlatVec3Modifiers KnockBackDirFlatAdd {get; protected set;} = new();
    protected PW_Shot _shot;
    /// <summary>
    /// The physical combat entity of this knocback's shot owner. Short for _shot.OwnerEntity.
    /// </summary>
    protected GE_IActiveCombatEntity OwnerEntity => _shot.OwnerEntity;
    /// <summary>
    /// The physical body of this knocback's shot owner. Short for _shot.OwnerEntity.Body.
    /// </summary>
    protected GB_IExternalBodyManager OwnerBody => OwnerEntity.Body;

    public override sealed void _Ready()
    {
        if (GetParent() is PW_Shot shot)
            _shot = shot;
        else
            GD.PushError("PW_KnockBack component has no PW_Shot parent.");
        ReadySpec();
    }

    /// <summary>
    /// Allows the implementing class to define further custom _Ready routines. <br/>
    /// <br/>
    /// This prevents the user from unintendedly hiding important PW_KnockBack's _Ready base routines. <br/>
    /// It is called after the PW_KnockBack base routines.
    /// </summary>
    protected virtual void ReadySpec(){}

    public abstract Vector3? KnockBackImpulse(Vector3 direction, float hitSize);
}