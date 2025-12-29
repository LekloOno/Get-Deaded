using Godot;

[GlobalClass]
public partial class PA_Dash : Node3D
{
    [Export] private AUD_Sound _sound;
    [Export] private PM_Dash _dash;

    public override void _Ready()
    {
        _dash.OnStart += (o, e) => _sound.Play();
    }
}