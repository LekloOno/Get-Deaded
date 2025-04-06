using Godot;

[GlobalClass]
public partial class PM_AirState : PM_SurfaceState
{
    [Export] float _slowMaxSpeed;       // The applied speed when the player speed is 0
    
    [Export] float _speedCurveStrength;
    [Export] float _slowAccel;       // The applied accel when the player speed is 0
    [Export] float _accelCurveStrength;
    [Export] float _concreteMaxSpeed;   // The threshold at which the applied MaxSpeed is CurrentData.MaxSpeed.

    public override Vector3 Accelerate(Vector3 wishDir, Vector3 velocity, float speedModifier, float delta)
    {
        float flatSpeed = MATH_Vector3Ext.Flat(velocity).Length();
        if (flatSpeed >= _concreteMaxSpeed)
            return base.Accelerate(wishDir, velocity, speedModifier, delta);

        float k = Mathf.Clamp(-1/_concreteMaxSpeed, 0, 1);

        float aSpeed = Mathf.Pow(k, _speedCurveStrength);
        float speed = (aSpeed * flatSpeed + 1) * (_slowMaxSpeed - CurrentData.MaxSpeed) + CurrentData.MaxSpeed;
        
        float aAccel = Mathf.Pow(k, _accelCurveStrength);
        float accel = (aAccel * flatSpeed + 1) * (_slowAccel - CurrentData.MaxAccel) + CurrentData.MaxAccel;

        return PHX_MovementPhysics.Acceleration(speed * speedModifier, accel, velocity, wishDir, delta);
    }
}