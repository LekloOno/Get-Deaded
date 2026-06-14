// Use to store external forces that should be applied somewhere in a physics body movement logic.
using Godot;
using System.Collections.Generic;
using System.Linq;

public class PHX_ForcesCache
{
    private List<Vector3> _impulseForces = [];      // Are automatically cleared by consuming them.
    private List<Vector3> _persistentForces = [];   // Can be consumed without clearing them, should be removed explicitly.
    
    public bool IsEmpty() => !_impulseForces.Any() && !_persistentForces.Any();

    public Vector3 Consume() => ConsumeImpulse() + ConsumePersistent();
    /// <summary>
    /// Persistent forces could typically be intended to be used additively to a velocity.
    /// They would thus need to be scaled with delta if such addition is performed every tick.
    /// </summary>
    /// <param name="delta"></param>
    /// <returns></returns>
    public Vector3 Consume(double delta) => ConsumeImpulse() + ConsumePersistent(delta);

    public Vector3 ConsumePersistent() => _persistentForces.Aggregate(Vector3.Zero, (acc, v) => acc + v);
    /// <summary>
    /// Persistent forces could typically be intended to be used additively to a velocity.
    /// They would thus need to be scaled with delta if such addition is performed every tick.
    /// </summary>
    /// <param name="delta"></param>
    /// <returns></returns>
    public Vector3 ConsumePersistent(double delta) => _persistentForces.Aggregate(Vector3.Zero, (acc, v) => acc + v) * (float)delta;
    
    public Vector3 ConsumeImpulse()
    {
        Vector3 sum = _impulseForces.Aggregate(Vector3.Zero, (acc, v) => acc + v);
        _impulseForces.Clear();
        return sum;
    }

    public void AddImpulse(Vector3 force) => _impulseForces.Add(force);
    public void AddPersistent(Vector3 force) => _persistentForces.Add(force);
    public bool RemovePersistent(Vector3 force) => _persistentForces.Remove(force);

    public void Clear()
    {
        _impulseForces.Clear();
        _persistentForces.Clear();
    }
}