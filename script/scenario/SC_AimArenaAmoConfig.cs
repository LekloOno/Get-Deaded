using Godot;

[GlobalClass]
public partial class SC_AimArenaAmoConfig : Area3D
{
    bool _infActive = false;
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("special_interaction"))
        {
            foreach (var body in GetOverlappingBodies())
            {
                
                if (body is PM_Controller controller)
                {
                    _infActive = !_infActive;
                    controller.WeaponsHandler.SetInfiniteMagazine(_infActive);
                    controller.WeaponsHandler.SetInfiniteAmmo(_infActive);
                    return;
                }
            }
        }
    }
}