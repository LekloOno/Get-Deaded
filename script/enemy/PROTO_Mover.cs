using Godot;
using System;

[GlobalClass]
public partial class PROTO_Mover : Node
{
    [Export] private PROTO_MoverData _data;
    public Node3D Target;

    private Vector3 _wishDir = Vector3.Zero;
    public Vector3 WishDir {get => _wishDir;}
    private Timer _straffeTimer;
    private Timer _speedTimer;
    private Random _rng = new Random();
    private float _speed = 1f;
    private bool _walking;

    public override void _Ready()
    {
        _wishDir.X = _rng.Next(2) * 2f - 1f;
        _straffeTimer = new();
        _straffeTimer.Timeout += ChangeStraffeDir;
        AddChild(_straffeTimer);
        StartStraffeTimer();

        _speedTimer = new();
        _speedTimer.Timeout += ChangeSpeed;
        AddChild(_speedTimer);
        StartSpeedTimer();
    }

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
        _wishDir.X *= -1;

        float seed = _rng.NextSingle();
        if (seed < _data.StraightProbability)
        {
            seed -= _data.StraightProbability / 2;
            _wishDir.Z = Math.Sign(seed);
        }
        else
            _wishDir.Z = 0;

        _wishDir = _wishDir.Normalized();
        
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
    

    public Vector3 GetAcceleration(Node3D self, Vector3 velocity, double delta)
    {
        Vector3 globalDir = self.GlobalBasis * _wishDir;
        Vector3 accel = PHX_MovementPhysics.Acceleration(_speed, _data.Acceleration, velocity, globalDir.Normalized(), (float)delta);
        return accel;
    }
}