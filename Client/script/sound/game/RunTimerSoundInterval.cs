using Godot;

[GlobalClass]
public partial class RunTimerSoundInterval : Resource
{
    [Export] public float TriggerTime   { get; private set; }
    [Export] public float BeepInterval  { get; private set; }
}