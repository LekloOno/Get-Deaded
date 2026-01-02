using Godot;

[GlobalClass]
public partial class PA_FullAuto : PA_Fire
{
    [Export] private PWF_FullAuto _fire;
    [Export] private AUD2_Sound _holdFire;
    [Export] private AUD2_Sound _tail;

    public override PW_Fire Fire => _fire;

    public override void _Ready()
    {
        base._Ready();

        _fire.Stopped += Release;
    }

    public override void ShotSound(object sender, int shots)
    {
        base.ShotSound(sender, shots);
        _holdFire?.Play();
        // Desub instead of a boolean - that will avoid countless branching
        _fire.Shot -= ShotSound;
    }

    private void Release()
    {
        _holdFire?.Stop();
        _tail?.Play();
        _fire.Shot += ShotSound;
    }
}