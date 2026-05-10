using Godot;

[GlobalClass]
public partial class UI_ScoreMarkShaker : Control
{
    [Export] private UI_ScoreMark _scoreMark; 
    [Export] public ANIM_Vec2TraumaLayer ShakeLayer;
    [Export] public float Trauma = 1f;
    [Export] public float ShakeIntensity = 10f;
    public string Text
    {
        get => _scoreMark.Text;
        set => _scoreMark.Text = value;
    }

    private double _time;

    public void Init()
    {
        _scoreMark.Init();
        ShakeLayer.AddTrauma(Trauma);
    }
    public override void _Process(double delta)
    {
        _time += delta;
        Position = ShakeLayer.GetShakeAngleIntensity((float) delta, (float) _time) * ShakeIntensity;
    }

    public void Fade()
    {
        _scoreMark.Fade();
    }

    public void Accumulate()
    {
        _scoreMark.Accumulate();
    }
}