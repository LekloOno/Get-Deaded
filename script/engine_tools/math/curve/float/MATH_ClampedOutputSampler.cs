using Godot;

public abstract partial class MATH_ClampedOutputSampler : MATH_FloatCurveSampler
{
    [Export] protected float _minOutput;
    [Export] protected float _maxOutput;

    public override float Sample(float value)
    {
        float ratio = GetRatio(value);
        return _maxOutput * ratio + _minOutput * (1 - ratio);
    }

    protected abstract float GetRatio(float value);
}