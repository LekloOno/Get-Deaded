using Godot;

[GlobalClass]
public partial class GL_Picker : Area3D
{
    [Export] private PW_WeaponsHandler _weaponsHandler;
    public override void _Ready()
    {
        AreaEntered += ProcessCollision;
    }

    private void ProcessCollision(Area3D area3D)
    {
        if (area3D is GL_IPickable pickable)
            pickable.GetPicked(this);
    }

    public void Disable()
    {
        CollisionMask = 0;
    }

    public void Enable()
    {
        CollisionMask = 5;
    }

    public bool PickAmmo()
    {
        // oui
        GD.Print("oui");
        return true;
    }
}