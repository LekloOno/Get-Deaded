using Godot;

[GlobalClass]
public partial class GL_Picker : Area3D
{
    [Export] private PW_WeaponsHandler _weaponsHandler;
    public override void _Ready()
    {
        BodyEntered += ProcessCollision;
        Enable();
    }

    private void ProcessCollision(Node body)
    {
        if (body is GL_IPickable pickable)
            pickable.GetPicked(this);
    }

    public void Disable()
    {
        CollisionMask = 0;
    }

    public void Enable()
    {
        CollisionMask = 0x10;
    }

    public bool PickAmmo(GL_AmmoData data)
    {
        // oui
        _weaponsHandler.PickAmmo(data);
        return true;
    }
}