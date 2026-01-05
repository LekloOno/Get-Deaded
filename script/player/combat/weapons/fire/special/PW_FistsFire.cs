using Godot;

[GlobalClass]
public partial class PW_FistsFire : PW_Fire
{
    [Export] private float _chargedKbMultiplier;
    [Export] private float _chargedDmgMultiplier;
    [Export] private ulong _chargeTime;
    private GB_ExternalBodyManagerWrapper _ownerBody;
    private ulong _chargeStartTime;

    public override void Disable() {}

    protected override void SpecInitialize(PC_Shakeable shakeableCamera, PC_Recoil recoilController, GB_ExternalBodyManagerWrapper ownerBody)
    {
        _ownerBody = ownerBody;
    }

    protected override bool SpecPress()
    {
        _chargeStartTime = PHX_Time.ScaledTicksMsec;
        return true;
    }

    protected override bool SpecRelease()
    {
        float chargeRatio = Mathf.Min((float) (PHX_Time.ScaledTicksMsec - _chargeStartTime) / _chargeTime, 1);
        float damage = chargeRatio * _chargedDmgMultiplier;
        float knockBack = chargeRatio * _chargedKbMultiplier;

        AddModifiers(knockBack, damage);

        bool didShoot = TryShoot();
        if (didShoot)
            _recoil?.Start();

        return didShoot;
    }

    private void AddModifiers(float knockBack, float damage)
    {
        foreach (PW_Shot shot in _shots)
        {
            shot.TempKnockBackMultiplier.Add(knockBack);
            shot.TempDamageMultiplier.Add(damage);
        }
    }
}