using System;
using Godot;

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
        _sprintInput.Start += StartSprint;
        _sprintInput.Stop += ResetSprint;
        
        _crouch.Started += SetDataCrouch;
        _crouch.Stopped += ResetData;

        _slide.Started += SetDataSlide;
        _slide.Stopped += ResetData;
    }

    private void StartSprint(object sender, EmptyInput args)
    {
        if (!IsCrouch() && !IsSlide())
            SetData(_stateData.Sprint);
    }

    private void ResetSprint(object sender, EmptyInput args)
    {
        if (!IsCrouch() && !IsSlide())
            SetData(_stateData.Normal);
    }


    private void ResetData()
    {
        if (_sprintInput.Active)
            SetData(_stateData.Sprint);
        else
            SetData(_stateData.Normal);
    }


    private void SetDataCrouch() => SetData(_stateData.Crouch);
    private void SetDataNormal() => SetData(_stateData.Normal);
    private void SetDataSlide() => SetData(_stateData.Slide);

    public void SetData(PM_SurfaceData data)
    {
        _currentData.OnStop?.Invoke();
        
        _currentData = data;
        data.OnStart?.Invoke();
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