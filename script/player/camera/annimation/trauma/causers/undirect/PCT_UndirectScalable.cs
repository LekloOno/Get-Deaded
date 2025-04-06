using Godot;

[GlobalClass]
public partial class PCT_UndirectScalable : PCT_Undirect
{
    [Export] private MATH_FloatCurveSampler _curveSampler;

    public PCT_UndirectScalable(){}
    public PCT_UndirectScalable(MATH_FloatCurveSampler curveSampler)
    {
        _curveSampler = curveSampler;
    }

    protected override float ProcessedAmount(PC_Shakeable shakeable) =>
        _curveSampler.Sample(shakeable.GlobalPosition.DistanceTo(GlobalPosition));
}