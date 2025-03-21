using System;
using Godot;
using Godot.Collections;



[GlobalClass]
public abstract partial class PW_Fire : Resource
{
    private const double _bufferMargin = 0.008;
    [Export] private Array<PW_Shot> _shots;
    [Export] protected float _spread = 0;
    [Export] protected ulong _fireRate;
    [Export] protected PW_Recoil _recoil;
    public PHX_AdditiveModifiers SpreadMultiplier {get; private set;} = new();
    public PHX_AdditiveModifiers RecoilMultiplier {get; private set;} = new();
    public EventHandler<ShotHitEventArgs> Hit;
    protected Node3D _sight;
    protected PC_DirectCamera _camera;
    private PC_Recoil _recoilController;

    protected ulong _lastShot = 0;
    
    private SceneTreeTimer _bufferTimer;
    private bool _pressBuffered = false;
    private bool _releaseBuffered = false;
    private static Random _random = new();

    public void Initialize(PC_DirectCamera camera, Node3D sight, Node3D _barel, PC_Recoil recoilController)
    {
        _camera = camera;
        _sight = sight;
        _recoilController = recoilController;
        foreach (PW_Shot shot in _shots)
        {
            shot.Initialize(_barel);
            shot.Hit += (o, e) => Hit?.Invoke(o, e);
        }
    }

    public ulong NextAvailableShot() => _fireRate + _lastShot - Time.GetTicksMsec();

    protected bool TryShoot()
    {
        if(!CanShoot())
            return false;

        Shoot();
        return true;
    }

    protected void Shoot()
    {
        _lastShot = Time.GetTicksMsec();

        SightTo(out Vector3 origin, out Vector3 direction);

        float spread = Mathf.Max(_spread * SpreadMultiplier.Result(), 0f);
        if (spread != 0)
        {
            Vector3 perp = _sight.GlobalBasis.X;
            float theta = (float)(_random.NextDouble() * 2.0 * Mathf.Pi);
            Vector3 rotationAxis = perp.Rotated(direction.Normalized(), theta).Normalized();

            float spreadAngle = Mathf.DegToRad((float)_random.NextDouble()*spread);
            direction = direction.Rotated(rotationAxis, spreadAngle);
        }
        
        foreach (PW_Shot shot in _shots)
            shot.Shoot(_sight, origin, direction);

        //_recoilController.AddRecoil(new(0.2f, 1f), true);
    }

    protected bool CanShoot() => Time.GetTicksMsec() - _lastShot >= _fireRate;

    private void SightTo(out Vector3 origin, out Vector3 direction)
    {
        origin = _sight.GlobalPosition;
        direction = -_sight.GlobalBasis.Z;
    }

    public void HandlePress()
    {
        ResetBuffer();

        if(Press())
            return;
        
        BufferPress();
    }

    public void HandleRelease()
    {
        ResetBuffer();

        if(Release())
            return;
        
        BufferRelease();
    }

    private void ResetBuffer()
    {
        if(_bufferTimer != null)
        {
            if (_pressBuffered)
            {
                _bufferTimer.Timeout -= SendPress;
                _pressBuffered = false;
            }
            else if (_releaseBuffered)
            {
                _bufferTimer.Timeout -= SendRelease;
                _releaseBuffered = false;
            }
        }
    }

    private void BufferPress()
    {
        _pressBuffered = true;
        _bufferTimer = _sight.GetTree().CreateTimer(NextAvailableShot()/1000.0 + _bufferMargin);
        _bufferTimer.Timeout += SendPress;
    }
    
    private void BufferRelease()
    {
        _releaseBuffered = true;
        _bufferTimer = _sight.GetTree().CreateTimer(NextAvailableShot()/1000.0 + _bufferMargin);
        _bufferTimer.Timeout += SendRelease;
    }

    private void SendPress()
    {
        _pressBuffered = false;
        Press();
    }
    private void SendRelease()
    {
        _releaseBuffered = false;
        Release();
    }

    /// <summary>
    /// Handle a down input. It would typically use TryShoot or Shoot for finer behaviors. It can also do nothing, or more complex behaviors.
    /// <para> It should return false when it results in an "unexpected" behavior. Example - if it is supposed to shoot but it was too soon to do so.</para>
    /// <para> It should return true when it handled the input expectedly. Example - if it is supposed to shoot, and it did shoot.</para>
    /// </summary>
    /// <returns>true if it was able to handle the input expectedly, false otherwise</returns>
    protected abstract bool Press();
    /// <summary>
    /// Handle an up input. It would typically use TryShoot or Shoot/CanShoot for finer behaviors. It can also do nothing or more complex behaviors.
    /// <para> It should return false when it results in an "unexpected" behavior. Example - if it is supposed to shoot but it was too soon to do so.</para>
    /// <para> It should return true when it handled the input expectedly. Example - if it is supposed to shoot, and it did shoot.</para>
    /// </summary>
    /// <returns>true if it was able to handle the input expectedly, false otherwise</returns>
    protected abstract bool Release();
    /// <summary>
    /// Handle the weapon disabling. Example - A weapons which shoots continuously should stop shooting on disable, even if no up input has been sent.
    /// </summary>
    public abstract void Disable();
}