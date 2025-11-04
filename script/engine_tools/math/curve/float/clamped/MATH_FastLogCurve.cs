using Godot;

[GlobalClass]
public partial class MATH_FastLogCurve : MATH_ClampedOutputSampler
{
    // https://www.desmos.com/calculator/oqfxjchzwx?lang=fr
    [Export] private float _halfInput;      // The input value at which half max ouput is reached.

    protected override float GetRatio(float value)
    {
        float divisor = value + _halfInput;
        if (divisor == 0f)
            return 1f;
            
        return Mathf.Clamp(value / divisor, 0f, 1f);
    }
}