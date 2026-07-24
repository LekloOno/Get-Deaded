using Godot;

[GlobalClass]
public abstract partial class SC_SpawnSector : SC_GenericSpawnerScript
{
    public SC_ParentSector? SectorParent { get; set; }
    public abstract SC_LeafSector? ActiveLeafSector();

    public SC_LeafSector? NextLeafSector()
    {
        SC_ParentSector? walker = SectorParent;

        while (walker != null)
        {
            if (walker.PendingNext != null)
                return walker.PendingNext.ActiveLeafSector();

            walker = walker.SectorParent;
        }

        return null;
    }
}