using System.IO;
using Godot;
using Godot.Collections;

[GlobalClass]
public abstract partial class PW_Fire : Resource
{
    [Export] private float _spread;
    [Export] private ulong _fireRate;
    [Export] private PW_Recoil _recoil;
    [Export] private Array<PW_Shot> _shots;
    public float SpreadMultiplier = 1f;
    public float RecoilMultiplier = 1f;
    private Camera3D _camera;
    private Node3D _sight;

    private ulong _lastShot = 0;

    public void Initialize(Camera3D camera, Node3D sight)
    {
        _camera = camera;
        _sight = sight;
    }

    protected bool TryShoot()
    {
        bool canShoot = Time.GetTicksMsec() - _lastShot > _fireRate;
        if (!canShoot)
            return false;

        _lastShot = Time.GetTicksMsec();

        SightTo(out Vector3 origin, out Vector3 direction);
        
        foreach (PW_Shot shot in _shots)
            shot.Shoot(origin, direction);
        
        return true;
    }

    private void SightTo(out Vector3 origin, out Vector3 direction)
    {
        origin = _sight.GlobalPosition;
        direction = -_sight.GlobalBasis.Z;
    }

    public abstract void Press();
    public abstract void Release();
}