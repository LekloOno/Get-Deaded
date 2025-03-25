using Godot;

public partial class GL_AmmoPickable(uint weaponIndex, uint fireIndex, uint ammos) : GL_PhysicsPickable(new GL_AmmoPickHandler())
{
    private uint _weaponIndex = weaponIndex;
    private uint _fireIndex = fireIndex;
    private uint _ammos = ammos;

    public override void GetPicked(GL_Picker picker)
    {
        if (_handler.HandlePick(picker))
            QueueFree();
    }
}