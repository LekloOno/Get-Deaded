using Godot;

[GlobalClass]
public abstract partial class SC_SpawnSector : SC_GenericSpawnerScript
{
    public override void _Ready()
    {
        ReadySpec();
    }
    protected abstract void ReadySpec();
}