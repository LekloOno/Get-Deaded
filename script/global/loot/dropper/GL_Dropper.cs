using System;
using Godot;

[GlobalClass]
public partial class GL_Dropper : Node3D
{
    [Export] private GL_DropTable _dropTable;
    [Export] private float _upImpulseForce;
    [Export] private float _sideImpulseForce;

    private Random _rng = new Random();

    public void Drop()
    {
        foreach (GL_DropItem item in _dropTable.Table)
        {
            float seed = (float)_rng.NextDouble();
            if (item.TryDrop(seed, out GL_PhysicsPickable pickable))
            {
                AddChild(pickable);
                pickable.Position = GlobalPosition;

                float seedImpulse = seed * 2f * Mathf.Pi;
                float xImpulse = Mathf.Cos(seedImpulse) * _sideImpulseForce;
                float zImpulse = Mathf.Sin(seedImpulse) * _sideImpulseForce;
                pickable.ApplyImpulse(new Vector3(xImpulse, _upImpulseForce, zImpulse));
            }
        }
    }
}