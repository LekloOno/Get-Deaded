using GaudioProcessTree;
using Godot;

[GlobalClass]
public partial class PA_InstantAction : Node3D
{
    [Export] private AUD_Sound _sound;
    [Export] private PM_Action _action;

    public override void _Ready() =>
        _action.OnStart += _sound.Play;
}