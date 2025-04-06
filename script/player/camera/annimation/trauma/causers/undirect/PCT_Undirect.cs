using Godot;
using Godot.Collections;

[GlobalClass]
public partial class PCT_Undirect : Area3D
{
    [Export] protected float _amount = 0.1f;

    public override void _Ready()
    {
        CollisionLayer = 0;
        CollisionMask = CONF_Collision.Masks.Trauma;
    }

    public void CauseTrauma()
    {
        Array<Area3D> traumaAreas = GetOverlappingAreas();
        foreach (Area3D area in traumaAreas)
        {
            if (area is PC_Shakeable shakeable)
                shakeable.AddTrauma(ProcessedAmount(shakeable));
        }
    }

    public void CauseClampedTrauma(float max)
    {
        Array<Area3D> traumaAreas = GetOverlappingAreas();
        foreach (Area3D area in traumaAreas)
        {
            if (area is PC_Shakeable shakeable)
                shakeable.AddClampedTrauma(ProcessedAmount(shakeable), max);
        }
    }

    protected virtual float ProcessedAmount(PC_Shakeable shakeable) => _amount;
}