using Godot;

public partial class VFX_SpeedWind : Node
{
    [ExportCategory("Settings")]
    [Export] private float _triggerSpeed;
    [Export] private float _fullSpeed;
    [Export] private float _lerpSpeed;

    [ExportCategory("Setup")]
    [Export] private VFX_SpeedWindController _wind = null!;
    [Export] private PM_Controller _controller = null!;

    public override void _Ready()
    {
        _wind.Intensity = 0;
    }

    public override void _PhysicsProcess(double delta)
    {
        float speed = _controller.Velocity.Length();
        float speedFactor = Mathf.Clamp((speed*3.6f-_triggerSpeed)/(_fullSpeed-_triggerSpeed), 0, 1);
        float next = Mathf.Lerp(_wind.Intensity, speedFactor, (float)delta * _lerpSpeed);

        _wind.Intensity = next;
    }
}