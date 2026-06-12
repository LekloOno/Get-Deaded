using GaudioProcessTree;
using Godot;

[GlobalClass]
public partial class PA_InstantAction : Node3D
{
    [Export] private AUD_Sound _sound = null!;
    [Export] private PM_Action _action = null!;

    public override void _Ready() =>
        _action.Started += _sound.Play;
}