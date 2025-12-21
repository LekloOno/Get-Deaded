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

    private ulong _scaledTicksMsec = 0;
    private ulong _scaledTicksUsec = 0;
    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Pausable;
    }

    public override void _PhysicsProcess(double delta)
    {
        double deltaMsec = delta * 1000;
        _scaledTicksMsec += (ulong) deltaMsec;
        _scaledTicksUsec += (ulong) deltaMsec * 1000;;
    }
    
}