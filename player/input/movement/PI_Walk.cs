using System;
using Godot;
using static PI_Direction;

[GlobalClass]
public partial class PI_Walk : PI_ActionHandler<Vector2>
{
    [Export] private Node3D _flatDirNode;

    public EventHandler OnStopOrBackward;
    public EventHandler<KeyPressedArgs> KeyPressed;
    public Vector3 SpaceWishDir {get; private set;}
    public Vector3 WishDir {get; private set;}
    public Vector2 WalkAxis {get; private set;}
    private Vector2 _nextWalkAxis;
    private bool _lastStopped = true;

    public override void _UnhandledInput(InputEvent @event)
    {
        WishDir = ComputeWishDir();

        if(IsStopped())
        {
            SetStop(); 
            OnStopOrBackward?.Invoke(this, EventArgs.Empty);
            return;
        } else
            SetStart();

        if(IsBacking())
            OnStopOrBackward?.Invoke(this, EventArgs.Empty);
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        WalkAxis = ComputeWalkAxis();
        
        if (@event.IsActionPressed(FORWARD)
            || @event.IsActionPressed(BACKWARD)
            || @event.IsActionPressed(LEFT)
            || @event.IsActionPressed(RIGHT))
        {
            KeyPressed?.Invoke(this, new KeyPressedArgs(WishDir, WalkAxis));
        }
    }

    public static Vector2 ComputeWalkAxis() => Input.GetVector(LEFT, RIGHT, FORWARD, BACKWARD);

    public bool IsStopped() => WalkAxis.X == 0 && WalkAxis.Y == 0;
    public bool IsBacking() => WalkAxis.Y > 0;
    public bool IsForwarding() => WalkAxis.Y < 0; // Not just !IsBacking, walk axis can't be 0

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

    private void SetStop()
    {
        if(_lastStopped)
            return;
        
        _lastStopped = true;
        Stop?.Invoke(this, WalkAxis);
    }

    private void SetStart()
    {
        if(!_lastStopped)
            return;
    
        _lastStopped = false;
        Start?.Invoke(this, WalkAxis);
    }

    protected override void HandleInput(InputEvent @event)
    {
        WalkAxis = ComputeWalkAxis();
        
        if (@event.IsActionPressed(FORWARD)
            || @event.IsActionPressed(BACKWARD)
            || @event.IsActionPressed(LEFT)
            || @event.IsActionPressed(RIGHT))
        {
            KeyPressed?.Invoke(this, new KeyPressedArgs(WishDir, WalkAxis));
        }
    }

    protected override void HandleExternal(PI_ActionState actionState, Vector2 value)
    {
        throw new NotImplementedException();
    }

    protected override Vector2 GetInputValue(InputEvent @event)
    {
        throw new NotImplementedException();
    }

    public override void EnableAction() => SetProcessUnhandledKeyInput(true);
    public override void DisableAction() => SetProcessUnhandledKeyInput(false);
}