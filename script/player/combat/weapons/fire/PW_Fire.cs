using System;
using Godot;
using Godot.Collections;

// Icon credits - Lorc - under CC BY 3.0 - https://lorcblog.blogspot.com/ - https://game-icons.net/1x1/lorc/gunshot.html
[GlobalClass, Icon("res://gd_icons/weapon_system/fire_icon.svg")]
public abstract partial class PW_Fire : WeaponComponent
{
    private const double BUFFER_MARGIN = 0.008;
    [Export] private Array<PW_Shot> _shots;
    [Export] protected float _spread = 0;
    [Export] protected ulong _fireRate;
    [Export] protected PW_Recoil _recoil;
    [Export] protected PW_Ammunition _ammos;
    [Export] protected uint _ammosPerShot = 1;
    [Export] protected uint _baseAmmos;
    protected Node3D _sight;

    [ExportCategory("Visuals")]
    [Export] public bool IsDerived;     // To indicate that this fire should be considered as a derived fire mode. Only usefull for ui, to not display this fire mode.
    [Export] public Texture2D Icon {get; private set;}
    [Export] private PCT_Fire _fireTraumaCauser;

    public MATH_AdditiveModifiers SpreadMultiplier {get; private set;} = new();
    public MATH_AdditiveModifiers RecoilMultiplier => _recoil.Modifier;
    public PW_Ammunition Ammos => _ammos;
    public EventHandler<ShotHitEventArgs> Hit;
    public EventHandler<int> Shot;      // Event arg is the amount of shots shot, most likely _shots.Count


    protected ulong _lastShot = 0;
    private SceneTreeTimer _bufferTimer;
    private bool _pressBuffered = false;
    private bool _releaseBuffered = false;
    private static Random _random = new();


    public void Initialize(PC_Shakeable shakeableCamera, Node3D sight, PC_Recoil recoilController, GB_ExternalBodyManager ownerBody)
    {
        if (_fireTraumaCauser != null)
        {
            _fireTraumaCauser.Initialize(shakeableCamera);
            Shot += _fireTraumaCauser.ShotTrauma;
        }
        
        _sight = sight;
        _ammos.Initialize(_sight.GetTree(), _baseAmmos);

        _recoil?.Initialize(recoilController);
        foreach (PW_Shot shot in _shots)
        {
            shot.Initialize(ownerBody);
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
            shot.Shoot(origin, direction);
        
        Shot?.Invoke(this, _shots.Count);
    }

    protected bool CanShoot() => Time.GetTicksMsec() - _lastShot >= _fireRate && _ammos.DidConsume(_ammosPerShot);

    private void SightTo(out Vector3 origin, out Vector3 direction)
    {
        origin = _sight.GlobalPosition;
        direction = -_sight.GlobalBasis.Z;
    }

    public bool Press()
    {
        ResetBuffer();

        if(SpecPress())
            return true;
        
        BufferPress();
        return false;
    }

    public bool Release()
    {
        ResetBuffer();

        if(SpecRelease())
            return true;
        
        BufferRelease();
        return false;
    }

    public void Reload()
    {
        _ammos.Reload();
    }

    public void ResetBuffer()
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
            _bufferTimer = _sight.GetTree().CreateTimer(NextAvailableShot()/1000.0 + BUFFER_MARGIN);
            _bufferTimer.Timeout += SendPress;
        }
    }
    
    private void BufferRelease()
    {
        _releaseBuffered = true;
        _bufferTimer = _sight.GetTree().CreateTimer(NextAvailableShot()/1000.0 + BUFFER_MARGIN);
        _bufferTimer.Timeout += SendRelease;
    }

    private void ReloadSendPress(object sender, EventArgs e) => SendPress();
    private void SendPress()
    {
        _pressBuffered = false;
        SpecPress();
    }
    private void SendRelease()
    {
        _releaseBuffered = false;
        SpecRelease();
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
    protected abstract bool SpecPress();
    /// <summary>
    /// Handle an up input. It would typically use TryShoot or Shoot/CanShoot for finer behaviors. It can also do nothing or more complex behaviors.
    /// <para> It should return false when it results in an "unexpected" behavior. Example - if it is supposed to shoot but it was too soon to do so.</para>
    /// <para> It should return true when it handled the input expectedly. Example - if it is supposed to shoot, and it did shoot.</para>
    /// </summary>
    /// <returns>true if it was able to handle the input expectedly, false otherwise</returns>
    protected abstract bool SpecRelease();
    /// <summary>
    /// Handle the weapon disabling. Example - A weapons which shoots continuously should stop shooting on disable, even if no up input has been sent.
    /// </summary>
    public abstract void Disable();
}