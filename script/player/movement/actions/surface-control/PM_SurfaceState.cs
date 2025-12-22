using System;
using Godot;

namespace Pew;

[GlobalClass]
public partial class PM_SurfaceState : Node
{
    [Export] private PM_SurfaceStateData _stateData;
    [Export] private PI_Sprint _sprintInput;
    [Export] private PM_Crouch _crouch;
    [Export] private PM_Slide _slide;
    private PM_SurfaceData _currentData;
    public PM_SurfaceData CurrentData => _currentData;

    public PM_SurfaceData Normal => _stateData.Normal;
    public PM_SurfaceData Sprint => _stateData.Sprint;
    public PM_SurfaceData Slide => _stateData.Slide;
    public PM_SurfaceData Crouch => _stateData.Crouch;

    public override void _Ready()
    {
        _currentData = _stateData.Normal;
        _sprintInput.Start += (o, e) => SetData(_stateData.Sprint);
        _sprintInput.Stop += (o, e) => SetData(_stateData.Normal);
        
        _crouch.OnStart += (o, e) => SetData(_stateData.Crouch);
        _crouch.OnStop += (o, e) => SetData(_stateData.Normal);

        _slide.OnStart += (o, e) => SetData(_stateData.Slide);
        _slide.OnStop += (o, e) => SetData(_stateData.Normal);
    }

    public void SetData(PM_SurfaceData data)
    {
        _currentData.OnStop?.Invoke(this, EventArgs.Empty);
        
        _currentData = data;
        data.OnStart?.Invoke(this, EventArgs.Empty);
    }

    public virtual Vector3 Accelerate(Vector3 wishDir, Vector3 velocity, float speedModifier, float delta)
    {
        float speed = CurrentData.MaxSpeed;
        float accel = CurrentData.MaxAccel;
        return PHX_MovementPhysics.Acceleration(speed * speedModifier, accel, velocity, wishDir, delta);
    }

    public bool IsNormal() => _currentData == _stateData.Normal;
    public bool IsSprint() => _currentData == _stateData.Sprint;
    public bool IsSlide() => _currentData == _stateData.Slide;
    public bool IsCrouch() => _currentData == _stateData.Crouch;
}