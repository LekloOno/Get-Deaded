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

    public override void _UnhandledInput(InputEvent @event) => WishDir = ComputeWishDir();
    public override void _UnhandledKeyInput(InputEvent @event) => HandleInput(@event);

    public bool IsStopped() => WalkAxis.X == 0 && WalkAxis.Y == 0;
    public bool IsBacking() => WalkAxis.Y > 0;
    public bool IsForwarding() => WalkAxis.Y < 0; // Not just !IsBacking, walk axis can't be 0
    public bool FowardDown() => Input.IsActionPressed(FORWARD);
    public bool BackwardDown() => Input.IsActionPressed(BACKWARD);
    public bool RightDown() => Input.IsActionPressed(RIGHT);
    public bool LeftDown() => Input.IsActionPressed(LEFT);
    public Vector3 FreeWishDir(Vector2 input) => _flatDirNode.Transform.Basis.Z * input.Y
                                                + _flatDirNode.Transform.Basis.X * input.X;

    public override void EnableAction()
    {
        SetProcessUnhandledKeyInput(true);
        SetProcessUnhandledInput(true);
    }
    public override void DisableAction()
    {
        SetProcessUnhandledKeyInput(false);
        SetProcessUnhandledInput(false);
        HandleExternal(PI_ActionState.STOPPED, Vector2.Zero);
    }

    protected override void HandleInput(InputEvent @event)
    {
        WalkAxis = ComputeWalkAxis();
        
        if (IsMovementKey(@event))
            KeyPressed?.Invoke(this, new KeyPressedArgs(WishDir, WalkAxis));

        if(IsStopped())
        {
            SetStop();
            return;
        } else
            SetStart();

        if(IsBacking())
            OnStopOrBackward?.Invoke(this, EventArgs.Empty);
    }
    public override void HandleExternal(PI_ActionState actionState, Vector2 value)
    {
        switch (actionState)
        {
            case PI_ActionState.STOPPED :
                WalkAxis = Vector2.Zero;
                WishDir = Vector3.Zero;
                SetStop();
                break;
            case PI_ActionState.STARTED :
                WalkAxis = value;
                WishDir = ComputeWishDir();
                SetStart();
                break;
            default :
                break;
        }
    }
    protected override Vector2 GetInputValue(InputEvent @event)
    {
        if (IsMovementKey(@event))
            return WalkAxis = ComputeWalkAxis();
        
        return Vector2.Zero;
    }

    private static Vector2 ComputeWalkAxis() => Input.GetVector(LEFT, RIGHT, FORWARD, BACKWARD);
    private static bool IsMovementKey(InputEvent @event) => @event.IsActionPressed(FORWARD)
                                                || @event.IsActionPressed(BACKWARD)
                                                || @event.IsActionPressed(LEFT)
                                                || @event.IsActionPressed(RIGHT);
    private Vector3 ComputeWishDir() => _flatDirNode.Transform.Basis.Z * WalkAxis.Y
                                        + _flatDirNode.Transform.Basis.X * WalkAxis.X;


    private void SetStop()
    {
        if(!_lastStopped)
        {
            _lastStopped = true;
            Stop?.Invoke(this, WalkAxis);
        }

        OnStopOrBackward?.Invoke(this, EventArgs.Empty);
    }

    private void SetStart()
    {
        if(!_lastStopped)
            return;
    
        _lastStopped = false;
        Start?.Invoke(this, WalkAxis);
    }
}