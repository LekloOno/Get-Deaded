// Use to store external forces that should be applied somewhere in a physics body movement logic.
using Godot;
using System.Collections.Generic;
using System.Linq;

public class PHX_ForcesCache
{
    private List<Vector3> impulseForces = new List<Vector3>();      // Are automatically consumed by using them.
    private List<Vector3> persistentForces = new List<Vector3>();   // Can be used without consumming them, should be removed explicitly.
    
    public bool IsEmpty() => !impulseForces.Any() && !persistentForces.Any();

    public Vector3 Consume() => ConsumeImpulse() + ConsumePersistent();

    public Vector3 ConsumePersistent() => persistentForces.Aggregate(Vector3.Zero, (acc, v) => acc + v);
    
    public Vector3 ConsumeImpulse()
    {
        Vector3 sum = impulseForces.Aggregate(Vector3.Zero, (acc, v) => acc + v);
        impulseForces.Clear();
        return sum;
    }

    public void AddImpulse(Vector3 force) => impulseForces.Add(force);
    public void AddPersistent(Vector3 force) => persistentForces.Add(force);
    public bool RemovePersistent(Vector3 force) => persistentForces.Remove(force);
}