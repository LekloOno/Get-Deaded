using GaudioProcessTree;
using Godot;

public partial class PA_FistsFire : Node
{
    private PW_FistsFire _fistsFire = null!;
    [Export] public AUD_Sound _shotSound = null!;
    [Export] public AUD_Sound _chargeStarted = null!;

    public override void _Ready()
    {
        if (GetParent() is not PW_FistsFire fistsFire)
        {
            GD.PushError($"[{nameof(PA_FistsFire)}] requires a [{nameof(PW_FistsFire)}] parent.");
            return;
        }

        _fistsFire = fistsFire;
        _fistsFire.ChargeStarted += OnChargeStarted;
        _fistsFire.Shot += OnShot;
    }

    private void OnShot(object? sender, int e) =>
        _shotSound.Play();

    private void OnChargeStarted(ulong obj) => 
        _chargeStarted.Play();
}