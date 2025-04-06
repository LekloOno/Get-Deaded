using Godot;

[GlobalClass]
public partial class MATH_LinearCurve : MATH_ClampedOutputSampler
{
    [Export] private float _minInput;
    [Export] private float _maxInput;

    protected override float GetRatio(float value) =>
        Mathf.Clamp(value - _minInput / (_maxInput - _minInput), 0f, 1f);
}