using System;
using Godot;
using static PI_Direction;

[GlobalClass]
public partial class PI_Walk : Node
{
    [Export] private Node3D _flatDirNode;

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

        if (@event.IsActionPressed(FORWARD)
            || @event.IsActionPressed(BACKWARD)
            || @event.IsActionPressed(LEFT)
            || @event.IsActionPressed(RIGHT))
        {
            KeyPressed?.Invoke(this, new KeyPressedArgs(WishDir, WalkAxis));
        }

        if(StopOrLess())
        {
            OnStopOrBackward?.Invoke(this, EventArgs.Empty);
        }
    }

    public static Vector2 ComputeWalkAxis() => Input.GetVector(LEFT, RIGHT, FORWARD, BACKWARD);

    public bool StopOrLess() => WalkAxis.Y > 0 || (WalkAxis.X == 0 && WalkAxis.Y == 0);

    public Vector3 FreeWishDir(Vector2 input)
    {
        return _flatDirNode.Transform.Basis.Z * input.Y + _flatDirNode.Transform.Basis.X * input.X;
    }

    public Vector3 ComputeWishDir()
    {
        return _flatDirNode.Transform.Basis.Z * WalkAxis.Y + _flatDirNode.Transform.Basis.X * WalkAxis.X;
    }

    public bool FowardDown() => Input.IsActionPressed(FORWARD);
    public bool BackwardDown() => Input.IsActionPressed(BACKWARD);
    public bool RightDown() => Input.IsActionPressed(RIGHT);
    public bool LeftDown() => Input.IsActionPressed(LEFT);
}