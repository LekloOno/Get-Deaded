using Godot;

public class PC_RecoilHandler
{
    private Vector2 _velocity = Vector2.Zero;
    private Vector2 _resistance = Vector2.Zero;
    private float _threshold;

    public PC_RecoilHandler(Vector2 angle, float time, float threshold)
    {
        _threshold = threshold;

        float radAngleX = Mathf.DegToRad(angle.X);
        float radAngleY = Mathf.DegToRad(angle.Y);

        float resX = 2f*radAngleX/Mathf.Pow(time, 2f);
        float resY = 2f*radAngleY/Mathf.Pow(time, 2f);
        _resistance = new(resX, resY);

        _velocity = _resistance * time;
    }
    
    public void AddVelocity(Vector2 velocity) => _velocity += velocity;
    public void AddCappedVelocity(Vector2 velocity, float max)
    {
        AddVelocity(velocity);
        if (_velocity.Length() > max)
            _velocity = _velocity.Normalized() * max;
    }
    public bool Tick(double delta, out Vector2 tickVelocity)
    {
        tickVelocity = _velocity * (float) delta;
        ApplyResistance(delta);
        return BelowThreshold();
    }

    protected void ApplyResistance(double delta) => _velocity -= _resistance * (float)delta;
    protected bool BelowThreshold() => _velocity.Length() < _threshold;
}