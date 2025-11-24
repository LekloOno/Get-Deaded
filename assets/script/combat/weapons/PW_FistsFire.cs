using Godot;

[GlobalClass]
public partial class PW_FistsFire : PW_Fire
{
    [Export] private float _chargedKbMultiplier;
    [Export] private float _chargedDmgMultiplier;
    [Export] private float _uppercutMaxKbMultiplier;
    [Export] private float _uppercutMinSpeed;
    [Export] private float _uppercutMaxSpeed;
    [Export] private float _speedKbMultiplier;
    [Export] private float _baseVerticalSpeed;
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
        float chargeRatio = Mathf.Min((float) (Time.GetTicksMsec() - _chargeStartTime) / _chargeTime, 1);
        float damage = chargeRatio * _chargedDmgMultiplier;
        float knockBack = chargeRatio * _chargedKbMultiplier;



        float verticalSpeed = _ownerBody.Velocity().Y;
        float verticalSpeedKM = verticalSpeed * 3.6f;
        bool uppercut = verticalSpeedKM > _uppercutMinSpeed;
        Vector3 dirKnockBack = Vector3.Zero;
        if (uppercut)
        {
            float uppercutRatio = (verticalSpeedKM - _uppercutMinSpeed) / (_uppercutMaxSpeed - _uppercutMinSpeed);
            float multiplier = 1 + uppercutRatio * _uppercutMaxKbMultiplier;
            dirKnockBack = new(0, multiplier*_baseVerticalSpeed*verticalSpeed, 0);

            AddDirKnockback(dirKnockBack);
        }
        else
        {
            float flatSpeed = (_ownerBody.Velocity() * new Vector3(1, 0, 1)).Length();
            float speedRatio = Mathf.Min(flatSpeed * 3.6f / _maxSpeed, 1);
            float multiplier = speedRatio * _speedKbMultiplier;

            knockBack += multiplier;
        }

        AddModifiers(knockBack, damage);

        bool didShoot = TryShoot();
        if (didShoot)
            _recoil?.Start();

        RemoveModifiers(knockBack, damage);
        if (uppercut)
            RemoveDirKnockback(dirKnockBack);

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

    private void AddDirKnockback(Vector3 dirKnockback)
    {
        foreach (PW_Shot shot in _shots)
            shot.KnockBackDirFlatAdd.Add(dirKnockback);
    }

    private void RemoveDirKnockback(Vector3 dirKnockback)
    {
        foreach (PW_Shot shot in _shots)
            shot.KnockBackDirFlatAdd.Remove(dirKnockback);
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