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
    [Export] protected PW_Ammunition _ammos;
    [Export] protected uint _ammosPerShot = 1;
    [Export] protected uint _baseAmmos;
    [Export] public bool IsDerived;     // To indicate that this fire should be considered as a derived fire mode. Only usefull for ui, to not display this fire mode.
    [Export] public Texture2D Icon {get; private set;}
    public PHX_AdditiveModifiers SpreadMultiplier {get; private set;} = new();
    public PHX_AdditiveModifiers RecoilMultiplier {get => _recoil.Modifier;}
    public EventHandler<ShotHitEventArgs> Hit;
    protected Node3D _sight;
    protected PC_DirectCamera _camera;

    protected ulong _lastShot = 0;
    
    private SceneTreeTimer _bufferTimer;
    private bool _pressBuffered = false;
    private bool _releaseBuffered = false;
    private static Random _random = new();

    public PW_Ammunition Ammos {get => _ammos;}

    public void Initialize(PC_DirectCamera camera, Node3D sight, Node3D _barel, PC_Recoil recoilController)
    {
        _camera = camera;
        _sight = sight;
        _ammos.Initialize(_sight.GetTree(), _baseAmmos);

        _recoil?.Initialize(recoilController);
        foreach (PW_Shot shot in _shots)
        {
            shot.Initialize(_barel);
            shot.Hit += (o, e) => Hit?.Invoke(o, e);
        }
    }

    public ulong NextAvailableShot() => _fireRate + _lastShot - Time.GetTicksMsec();

    public bool PickAmmo(int amount, bool magazine)
    {
        if (amount == 0)
            return true;
        
        if (amount > 0)
            return _ammos.FillAmos((uint)amount, magazine) > 0;
        
        return _ammos.EmptyAmos((uint)-amount, magazine) > 0;
    }

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
    }

    protected bool CanShoot() => Time.GetTicksMsec() - _lastShot >= _fireRate && _ammos.DidConsume(_ammosPerShot);

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

    public void Reload()
    {
        _ammos.Reload();
    }

    private void ResetBuffer()
    {
        if (_pressBuffered)
        {
            if (_bufferTimer == null)
                _ammos.ReloadCompleted -= ReloadSendPress;
            else
                _bufferTimer.Timeout -= SendPress;
            
            _pressBuffered = false;
        }
        else if (_releaseBuffered && _bufferTimer != null)
        {
            _bufferTimer.Timeout -= SendRelease;
            _releaseBuffered = false;
        }
    }

    private void BufferPress()
    {
        _pressBuffered = true;
        if (_ammos.IsReloading)
        {
            _ammos.ReloadCompleted += ReloadSendPress;
        }
        else
        {
            _bufferTimer = _sight.GetTree().CreateTimer(NextAvailableShot()/1000.0 + _bufferMargin);
            _bufferTimer.Timeout += SendPress;
        }
    }
    
    private void BufferRelease()
    {
        _releaseBuffered = true;
        _bufferTimer = _sight.GetTree().CreateTimer(NextAvailableShot()/1000.0 + _bufferMargin);
        _bufferTimer.Timeout += SendRelease;
    }

    private void ReloadSendPress(object sender, EventArgs e) => SendPress();
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

    public bool CanReload(out bool tactical)
    {
        tactical = _ammos.LoadedAmos > 0;
        return _ammos.CanReload();
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