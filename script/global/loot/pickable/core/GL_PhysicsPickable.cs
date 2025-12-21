using Godot;

public partial class GL_PhysicsPickable(GL_IPickHandler handler, float horizontalDamp, float lifeTime) : RigidBody3D, GL_IPickable
{
    private GL_IPickHandler _handler = handler;
    private float _horizontalDamp = horizontalDamp;

    public override void _Ready()
    {
        if (lifeTime <= 0)
            return;

        GetTree().CreateTimer(lifeTime, false, true).Timeout += QueueFree;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 damp = new Vector3(-LinearVelocity.X * _horizontalDamp, 0, -LinearVelocity.Z * _horizontalDamp);
        ApplyForce(damp);
    }
    public void GetPicked(GL_Picker picker)
    {
        if (_handler.HandlePick(picker))
            QueueFree();
    }
}