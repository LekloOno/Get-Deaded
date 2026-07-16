using Godot;

[GlobalClass]
public abstract partial class SC_SpawnSector : SC_GenericSpawnerScript
{
    public abstract SC_LeafSector? ActiveLeafSector();
    public override void _Ready()
    {
        ReadySpec();
    }
    protected abstract void ReadySpec();
}