using Godot;

[GlobalClass]
public partial class PWK_Velocity : PWK_Simple
{
    [Export] private float _uppercutMaxKbMultiplier;
    [Export] private float _uppercutMinSpeed;
    [Export] private float _uppercutMaxSpeed;
    [Export] private float _speedKbMultiplier;
    [Export] private float _baseVerticalSpeed;
    [Export] private float _maxSpeed;
    private ulong _chargeStartTime;

    protected override Vector3 BaseKnockBackImpulse(Vector3 direction, float hitSize)
    {
        Vector3 impulse = base.BaseKnockBackImpulse(direction, hitSize);

        float verticalSpeed = OwnerBody.Velocity().Y;
        float verticalSpeedKM = verticalSpeed * 3.6f;

        Vector3 dirKnockBack = Vector3.Zero;

        if (IsUppercut(verticalSpeedKM))
        {
            float uppercutRatio = (verticalSpeedKM - _uppercutMinSpeed) / (_uppercutMaxSpeed - _uppercutMinSpeed);
            float multiplier = 1 + uppercutRatio * _uppercutMaxKbMultiplier;
            dirKnockBack = new(0, multiplier*_baseVerticalSpeed*verticalSpeed, 0);

            impulse += dirKnockBack;
        }
        else
        {
            float flatSpeed = (OwnerBody.Velocity() * new Vector3(1, 0, 1)).Length();
            float speedRatio = Mathf.Min(flatSpeed * 3.6f / _maxSpeed, 1);
            float multiplier = speedRatio * _speedKbMultiplier;

            impulse *= multiplier;
        }

        return impulse;
    }

    private bool IsUppercut(float verticalSpeedKM) => verticalSpeedKM > _uppercutMinSpeed;
}