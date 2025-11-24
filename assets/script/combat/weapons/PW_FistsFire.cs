using Godot;

[GlobalClass]
public partial class PW_FistsFire : PW_Fire
{
    [Export] private float _chargedKbMultiplier;
    [Export] private float _chargedDmgMultiplier;
    [Export] private float _speedKbMultiplier;
    [Export] private float _maxSpeed;
    [Export] private ulong _chargeTime;
    private GB_ExternalBodyManager _ownerBody;
    private ulong _chargeStartTime;

    public override void Disable() {}

    protected override void SpecInitialize(PC_Shakeable shakeableCamera, PC_Recoil recoilController, GB_ExternalBodyManager ownerBody)
    {
        _ownerBody = ownerBody;
    }

    protected override bool SpecPress()
    {
        _chargeStartTime = Time.GetTicksMsec();
        return true;
    }

    protected override bool SpecRelease()
    {
        float ratio = Mathf.Min((float) (Time.GetTicksMsec() - _chargeStartTime) / _chargeTime, 1);

        float speed = (_ownerBody.Velocity() * new Vector3(1, 0, 1)).Length();
        float velocityBoost = Mathf.Min(speed * 3.6f / _maxSpeed, 1) * _speedKbMultiplier;
        
        float knockBack = ratio*_chargedKbMultiplier + velocityBoost;
        float damage = ratio*_chargedDmgMultiplier;

        AddModifiers(knockBack, damage);

        bool didShoot = TryShoot();
        if (didShoot)
            _recoil?.Start();

        RemoveModifiers(knockBack, damage);
        return didShoot;
    }

    private void AddModifiers(float knockBack, float damage)
    {
        foreach (PW_Shot shot in _shots)
        {
            shot.KnockBackMultiplier.Add(knockBack);
            shot.DamageMultipler.Add(damage);
        }
    }

    private void RemoveModifiers(float knockBack, float damage)
    {
        foreach (PW_Shot shot in _shots)
        {
            shot.KnockBackMultiplier.Remove(knockBack);
            shot.DamageMultipler.Remove(damage);
        }
    }
}