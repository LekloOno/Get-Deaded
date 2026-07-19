using Godot;

[GlobalClass]
public abstract partial class SC_SpawnSector : SC_GenericSpawnerScript
{
    public abstract SC_LeafSector? ActiveLeafSector();
}