using Godot;

[GlobalClass]
public partial class UI_GlobalTimeTracker : Label
{
    private double _time = 0;

    public override void _Ready()
    {
        SetProcess(false);
    }

    public void OnGameStart()
    {
        _time = 0;
        SetProcess(true);
    }

    public override void _Process(double delta)
    {
        _time += delta;
        Text = Mathf.Round(_time) + "";
    }

    public void OnGameEnd()
    {
        SetProcess(false);
    }
}