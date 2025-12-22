using Godot;

namespace Pew;

[GlobalClass]
public partial class PA_SpeedWind : Node
{
    [ExportCategory("Settings")]
    [Export] private float _maxVolume;
    [Export] private float _triggerSpeed;
    [Export] private float _fullSpeed;
    [Export] private float _lerpSpeed;

    [ExportCategory("Setup")]
    [Export] private PM_Controller _controller;
    [Export] private AudioStreamPlayer2D _wind;

    public override void _Ready()
    {
        _wind.VolumeDb = -80;
    }

    public override void _PhysicsProcess(double delta)
    {
        float speed = _controller.Velocity.Length();
        float speedFactor = Mathf.Clamp((speed*3.6f-_triggerSpeed)/(_fullSpeed-_triggerSpeed), 0, 1);
        
        float maxLinear = Mathf.DbToLinear(_maxVolume);

        float currentLinear = Mathf.DbToLinear(_wind.VolumeDb);
        float next = Mathf.Lerp(currentLinear, speedFactor * maxLinear, (float)delta * _lerpSpeed);

        _wind.VolumeDb = Mathf.LinearToDb(next);
    }
}