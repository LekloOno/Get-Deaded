using Godot;

public abstract partial class GL_PhysicsPickable(GL_IPickHandler handler) : RigidBody3D, GL_IPickable
{
    protected GL_IPickHandler _handler = handler;
    public abstract void GetPicked(GL_Picker picker);
}