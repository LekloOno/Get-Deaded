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

    /// <summary>
    /// Gives a ratio from _minOutput to _maxOuput, which means the return value should be between 0 and 1.
    /// </summary>
    /// <param name="value">the input value</param>
    /// <returns>The output value ratio, between 0 and 1</returns>
    protected abstract float GetRatio(float value);
}