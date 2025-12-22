using Godot;
using System;

[GlobalClass]
public partial class PROTO_Mover : Node
{
    [Export] private PROTO_MoverData _data;
    [Export] private Node3D _dirNode;
    public Node3D Target;

    private Vector2 _walkAxis = Vector2.Zero;
    public Vector2 WalkAxis {get => _walkAxis;}
    public Vector3 WishDir => ComputeWishDir();
    private Timer _straffeTimer;
    private Timer _speedTimer;
    private readonly Random _rng = new Random();
    private float _speed = 1f;
    private bool _walking;

    public override void _Ready()
    {
        _walkAxis.X = _rng.Next(2) * 2f - 1f;
        _straffeTimer = new()
        {
            ProcessMode = ProcessModeEnum.Pausable,
            ProcessCallback = Timer.TimerProcessCallback.Physics
        };
        _straffeTimer.Timeout += ChangeStraffeDir;
        AddChild(_straffeTimer);
        StartStraffeTimer();

        _speedTimer = new()
        {
            ProcessMode = ProcessModeEnum.Pausable,
            ProcessCallback = Timer.TimerProcessCallback.Physics
        };
        _speedTimer.Timeout += ChangeSpeed;
        AddChild(_speedTimer);
        StartSpeedTimer();
    }

    private Vector3 ComputeWishDir() => _dirNode.Transform.Basis.Z * WalkAxis.Y
                                        + _dirNode.Transform.Basis.X * WalkAxis.X;

    public void Rotate(Node3D self)
    {
        if (Target == null)
            return;

        Vector3 target = Target.GlobalPosition;
        target.Y = self.GlobalPosition.Y;

        self.LookAt(target);
    }

    public void ChangeStraffeDir()
    {
        _walkAxis.X *= -1;

        float seed = _rng.NextSingle();
        if (seed < _data.StraightProbability)
        {
            seed -= _data.StraightProbability / 2;
            _walkAxis.Y = Math.Sign(seed);
        }
        else
            _walkAxis.Y = 0;

        _walkAxis = _walkAxis.Normalized();
        
        StartStraffeTimer();
    }

    private void StartStraffeTimer()
    {
        float seed = _rng.NextSingle();
        float nextStaffe = Mathf.Lerp(_data.MinStraffe, _data.MaxStraffe, seed); 
        _straffeTimer.Start(nextStaffe);
    }

    public void ChangeSpeed()
    {
        float seed = _rng.NextSingle();
        _speed = PickSpeed(seed);
        StartSpeedTimer();
    }

    public float PickSpeed(float seed)
    {
        if (seed < _data.PropWalk)
            return _data.WalkSpeed;
        
        seed -= _data.PropWalk;
        
        if (seed < _data.PropRun)
            return _data.RunSpeed;

        return _data.SprintSpeed;
    }

    public void StartSpeedTimer()
    {
        float seed = _rng.NextSingle();
        float nextChange = Mathf.Lerp(_data.MinHold, _data.MaxHold, seed);
        _speedTimer.Start(nextChange);
    }
    

    public Vector3 GetAcceleration(Vector3 velocity, double delta)
    {
        Vector3 accel = PHX_MovementPhysics.Acceleration(_speed, _data.Acceleration, velocity, WishDir.Normalized(), (float)delta);
        return accel;
    }
}