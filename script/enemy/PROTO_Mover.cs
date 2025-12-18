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
    private Random _rng = new Random();

    public override void _Ready()
    {
        _wishDir.X = _rng.Next(2) * 2f - 1f;
        _straffeTimer = new();
        _straffeTimer.Timeout += ChangeStraffeDir;
        AddChild(_straffeTimer);
        StartStraffeTimer();
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
        StartStraffeTimer();
    }

    private void StartStraffeTimer()
    {
        float seed = (float)_rng.NextSingle();
        float nextStaffe = Mathf.Lerp(_data.MinStraffe, _data.MaxStraffe, seed); 
        _straffeTimer.Start(nextStaffe);
    }
    

    public Vector3 GetAcceleration(Node3D self, Vector3 velocity, double delta)
    {
        Vector3 globalDir = self.GlobalBasis * _wishDir;
        Vector3 accel = PHX_MovementPhysics.Acceleration(_data.Speed, _data.Acceleration, velocity, globalDir.Normalized(), (float)delta);
        return accel;
    }
}