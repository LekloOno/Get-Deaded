using Godot;

public interface VFX_ITrailMesh: PHX_ListenPoolObject<VFX_ITrailMesh>
{     
    abstract void Shoot(Vector3 origin, Vector3 hit);
}