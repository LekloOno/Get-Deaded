using Godot;
using Godot.Collections;

[GlobalClass]
public partial class PC_TraumaCauser : Area3D
{
    [Export] private float _amount = 0.1f;
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
                shakeable.AddTrauma(_amount);
        }
    }
}