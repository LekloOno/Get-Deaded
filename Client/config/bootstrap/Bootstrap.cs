using GaudioProcessTree.Static.Time;
using Godot;

public partial class Bootstrap : Node
{
    class PHX_LocalTimeWrapper : AUD_ILocalTime
    {
        public ulong LocalScaledTicksMsec => PHX_Time.ScaledTicksMsec;
        public ulong LocalScaledTicksUsec => PHX_Time.ScaledTicksUsec;
    }

    public async override void _EnterTree()
    {
        await StaticServiceLifeCycle<PHX_Time>.Initialized;
        AUD_Time.Instance = new PHX_LocalTimeWrapper();
    }
}