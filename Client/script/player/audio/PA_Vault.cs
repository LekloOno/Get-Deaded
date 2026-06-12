using GaudioProcessTree;
using Godot;

public partial class PA_Vault : Node
{
    [ExportCategory("Setup")]
    [Export] private AUD_Sound _sound = null!;
    [Export] private PM_LedgeClimb _ledgeClimb = null!;

    public override void _Ready()
    {
        _ledgeClimb.SuperGlideStarted += _sound.Play;
    }
}