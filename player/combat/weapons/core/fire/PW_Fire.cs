using System.IO;
using Godot;
using Godot.Collections;

[GlobalClass]
public abstract partial class PW_Fire : Resource
{
    [Export] private Array<PW_Shot> _shots;
    [Export] protected float _spread;
    [Export] protected ulong _fireRate;
    [Export] protected PW_Recoil _recoil;
    public float SpreadMultiplier = 1f;
    public float RecoilMultiplier = 1f;
    protected Node3D _sight;
    protected Camera3D _camera;

    protected ulong _lastShot = 0;

    public void Initialize(Camera3D camera, Node3D sight)
    {
        _camera = camera;
        _sight = sight;
        foreach (PW_Shot shot in _shots)
            shot.Initialize();
    }

    protected void Shoot()
    {
        _lastShot = Time.GetTicksMsec();

        SightTo(out Vector3 origin, out Vector3 direction);
        
        foreach (PW_Shot shot in _shots)
            shot.Shoot(_sight.GetWorld3D(), origin, direction);
    }

    protected bool CanShoot() => Time.GetTicksMsec() - _lastShot > _fireRate;

    private void SightTo(out Vector3 origin, out Vector3 direction)
    {
        origin = _sight.GlobalPosition;
        direction = -_sight.GlobalBasis.Z;
    }

    public abstract void Press();
    public abstract void Release();
}