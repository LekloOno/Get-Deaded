using GaudioProcessTree;
using Godot;

public partial class PA_PickAmmos : Node3D
{
    [Export] private AUD_Sound _sound;
    [Export] private PW_WeaponsHandler _weaponsHandler;

    public override void _Ready()
    {
        _weaponsHandler.AmmosPicked += _sound.Play;
    }
}