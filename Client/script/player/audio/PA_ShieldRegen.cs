using GaudioProcessTree;
using Godot;

public partial class PA_ShieldRegen : Node
{
    [Export] private AUD_Sound _breakSound = null!;
    [Export] private AUD_Sound _regenSound = null!;
    [Export] private GC_Shield _shield = null!;

    public override void _Ready()
    {
        _shield.RegenTimer.Timeout += _regenSound.Play;
        _shield.OnFull += (_, _) => _regenSound.Stop();
        _shield.OnDamage += (_, _) => _regenSound.Stop();
        _shield.OnBreak += (_, _) => _breakSound.Play();
    }
}