using GaudioProcessTree;
using Godot;

public partial class PA_ShieldRegen : Node
{
    [Export] private AUD_Sound _sound;
    [Export] private GC_Shield _shield;

    public override void _Ready()
    {
        _shield.RegenTimer.Timeout += _sound.Play;
        _shield.OnFull += (_, _) => _sound.Stop();
        _shield.OnDamage += (_, _) => _sound.Stop();
    }
}