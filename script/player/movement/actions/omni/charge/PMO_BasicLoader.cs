using Godot;

[GlobalClass]
public partial class PMO_BasicLoader : Node, PM_IOmniLoader
{
    [Export] private float _chargePerSec = 5f;
    [Export] private float _consumeStale = 0.2f;
    [Export] private float _tryConsumeStale = 0f;

    private Timer _staleTimer;

    public PM_OmniCharge Charge {private get; set;}

    public override void _Ready()
    {
        _staleTimer = new() {
            OneShot = true,
            ProcessMode = ProcessModeEnum.Pausable,
            ProcessCallback = Timer.TimerProcessCallback.Physics
        };

        AddChild(_staleTimer);
        _staleTimer.Timeout += StartLoading;
    }

    public override void _PhysicsProcess(double delta)
    {
        Charge.Current += _chargePerSec * (float)delta;
        if (Charge.Current >= Charge.Max)
            SetPhysicsProcess(false);
    }

    public void Consumed(float charge) =>
        Stale(_consumeStale);

    public void TriedConsume(float charge) =>
        Stale(_tryConsumeStale);

    private void Stale(float time)
    {
        if (time <= 0f)
        {
            StartLoading();
            return;
        }

        _staleTimer.Start(time);
    }

    private void StartLoading() => SetPhysicsProcess(true);
}