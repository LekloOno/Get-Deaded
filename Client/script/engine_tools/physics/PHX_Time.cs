using Godot;

public partial class PHX_Time : Node
{
    public static PHX_Time Instance {get; private set;}
    /// <summary>
    /// The scaled time (pause and time scale-aware) elapsed since the start of the engine in Miliseconds. <br/>
    /// <br/>
    /// Should be used in _PhysicsProcess. Any logic that requires scaled time in _Process can probably rely on tweens or timer instead. 
    /// </summary>
    public static ulong ScaledTicksMsec {get => Instance._scaledTicksMsec;}
    /// <summary>
    /// The scaled time (pause and time scale-aware) elapsed since the start of the engine in Microseconds. <br/>
    /// <br/>
    /// Should be used in _PhysicsProcess. Any logic that requires scaled time in _Process can probably rely on tweens or timer instead. 
    /// </summary>
    public static ulong ScaledTicksUsec {get => Instance._scaledTicksUsec;}

    public ulong LocalScaledTicksMsec => _scaledTicksMsec;

    public ulong LocalScaledTicksUsec => _scaledTicksUsec;

    private ulong _scaledTicksMsec = 0;
    private ulong _scaledTicksUsec = 0;
    public override void _EnterTree()
    {
        Instance = this;
        StaticServiceLifeCycle<PHX_Time>.MarkInitialized();
    }

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Pausable;
    }

    private double _unaccumulatedDeltaMsec = 0;
    private double _unaccumulatedDeltaUsec = 0;
    public override void _PhysicsProcess(double delta)
    {
        double deltaMsec = delta * 1000;
        double deltaUsec = deltaMsec * 1000;

        deltaMsec += _unaccumulatedDeltaMsec;
        deltaUsec += _unaccumulatedDeltaUsec;

        _scaledTicksMsec += (ulong) deltaMsec;
        _scaledTicksUsec += (ulong) deltaUsec;

        _unaccumulatedDeltaMsec = deltaMsec % 1;
        _unaccumulatedDeltaUsec = deltaUsec % 1;
    }
}