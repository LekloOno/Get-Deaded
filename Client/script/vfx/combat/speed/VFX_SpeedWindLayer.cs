using Godot;

[GlobalClass, Tool]
public partial class VFX_SpeedWindLayer : Control
{
    [Export] private float _speed = 4f;
    [Export] private float _frequency = 0.1f;

    [Export(PropertyHint.Range, "0,1")]
    public  float Intensity;
    
    private double _phase = 0.0;

    public override void _EnterTree()
    {
        SetProcess(true);
    }

    public override void _Process(double delta)
    {
        float effectiveSpeed = Mathf.Lerp(0.2f, 1.0f, Intensity) * _speed;
        float effectiveFrequency = Mathf.Lerp(0.1f, 1.0f, Intensity) * _frequency;

        _phase += effectiveSpeed * effectiveFrequency * delta;

        SetInstanceShaderParameter("intensity", Intensity);
        SetInstanceShaderParameter("phase", (float)_phase);
        SetInstanceShaderParameter("manual_frequency", effectiveFrequency);
    }
}