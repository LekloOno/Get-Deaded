using System;
using Godot;

[GlobalClass]
public partial class PI_Walk : Node
{
    [Export] public Node3D FlatDirNode;
    [Export] public Node3D SightPositionNode;

    public EventHandler OnStopOrBackward;
    public EventHandler<KeyPressedArgs> KeyPressed;
    public Vector3 SpaceWishDir {get; private set;}
    public Vector3 WishDir {get; private set;}
    public Vector2 WalkAxis {get; private set;}
    private Vector2 _nextWalkAxis;

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        WalkAxis = ComputeWalkAxis();
        WishDir = ComputeWishDir();

        if (@event.IsActionPressed("move_forward")
            || @event.IsActionPressed("move_backward")
            || @event.IsActionPressed("move_left")
            || @event.IsActionPressed("move_right"))
        {
            KeyPressed?.Invoke(this, new KeyPressedArgs(WishDir, WalkAxis));
        }

        if(StopOrLess())
        {
            OnStopOrBackward?.Invoke(this, EventArgs.Empty);
        }
    }

    public static Vector2 ComputeWalkAxis()
    {
        return Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
    }

    public bool StopOrLess()
    {
        GD.Print(WalkAxis);
        return WalkAxis.Y > 0 || (WalkAxis.X == 0 && WalkAxis.Y == 0);
    }

    public Vector3 FreeWishDir(Vector2 input)
    {
        return FlatDirNode.Transform.Basis.Z * input.Y + FlatDirNode.Transform.Basis.X * input.X;
    }

    public Vector3 ComputeWishDir()
    {
        return FlatDirNode.Transform.Basis.Z * WalkAxis.Y + FlatDirNode.Transform.Basis.X * WalkAxis.X;
    }
}