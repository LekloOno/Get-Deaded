using Godot;

public partial class GL_PhysicsPickable(GL_IPickHandler handler) : RigidBody3D, GL_IPickable
{
    private GL_IPickHandler _handler = handler;
    public void GetPicked(GL_Picker picker)
    {
        if (_handler.HandlePick(picker))
            QueueFree();
    }
}