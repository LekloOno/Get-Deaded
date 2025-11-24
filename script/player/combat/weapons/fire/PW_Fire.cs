using System;
using Godot;
using Godot.Collections;

/// <summary>
/// Handles the firing behavior of its Shots.
/// <para>
/// It handles input buffering, and allows to set custom fire rythms and modes such as a full-auto, burst or semi-auto fire.
/// </para>
/// </summary>

// Icon credits - Lorc - under CC BY 3.0 - https://lorcblog.blogspot.com/ - https://game-icons.net/1x1/lorc/gunshot.html
[GlobalClass, Icon("res://gd_icons/weapon_system/fire_icon.svg")]
public abstract partial class PW_Fire : WeaponComponent
{
    private const double BUFFER_MARGIN = 0.008;
    [Export] protected Array<PW_Shot> _shots;
    [Export] protected ulong _fireRate;
    [Export] protected PW_Recoil _recoil;
    [Export] protected PW_Ammunition _ammos;
    [Export] protected uint _ammosPerShot = 1;

    [ExportCategory("Visuals")]
    [Export] public bool IsDerived;     // To indicate that this fire should be considered as a derived fire mode. Only usefull for ui, to not display this fire mode.
    [Export] public Texture2D Icon {get; private set;}
    [Export] private PCT_Fire _fireTraumaCauser;

    public MATH_AdditiveModifiers RecoilMultiplier => _recoil.Modifier;
    public PW_Ammunition Ammos => _ammos;
    public EventHandler<ShotHitEventArgs> Hit;
    public EventHandler<int> Shot;      // Event arg is the amount of shots shot, most likely _shots.Count


    protected ulong _lastShot = 0;
    private SceneTreeTimer _bufferTimer;
    private bool _pressBuffered = false;
    private bool _releaseBuffered = false;
    private static Random _random = new();


    public void Initialize(PC_Shakeable shakeableCamera, PC_Recoil recoilController, GB_ExternalBodyManager ownerBody)
    {
        if (_fireTraumaCauser != null)
        {
            _fireTraumaCauser.Initialize(shakeableCamera);
            Shot += _fireTraumaCauser.ShotTrauma;
        }
        
        _ammos.Initialize();

        _recoil?.Initialize(recoilController);
        SpecInitialize(shakeableCamera, recoilController, ownerBody);

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
            return _ammos.FillAmmos((uint)amount, magazine) > 0;
        
        return _ammos.EmptyAmmos((uint)-amount, magazine) > 0;
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
        
        foreach (PW_Shot shot in _shots)
            shot.Shoot();
        
        Shot?.Invoke(this, _shots.Count);
    }

    protected bool CanShoot() => Time.GetTicksMsec() - _lastShot >= _fireRate && _ammos.DidConsume(_ammosPerShot);

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
            _bufferTimer = GetTree().CreateTimer(NextAvailableShot()/1000.0 + BUFFER_MARGIN);
            _bufferTimer.Timeout += SendPress;
        }
    }
    
    private void BufferRelease()
    {
        _releaseBuffered = true;
        _bufferTimer = GetTree().CreateTimer(NextAvailableShot()/1000.0 + BUFFER_MARGIN);
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
        tactical = _ammos.LoadedAmmos > 0;
        return _ammos.CanReload();
    }

    public void AddSpread(float multiplier)
    {
        foreach (PW_Shot shot in _shots)
            shot.SpreadMultiplier?.Add(multiplier);
    }

    public void RemoveSpread(float multiplier)
    {
        foreach (PW_Shot shot in _shots)
            shot.SpreadMultiplier?.Remove(multiplier);
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
    protected abstract void SpecInitialize(PC_Shakeable shakeableCamera, PC_Recoil recoilController, GB_ExternalBodyManager ownerBody);
    /// <summary>
    /// Handle the weapon disabling. Example - A weapons which shoots continuously should stop shooting on disable, even if no up input has been sent.
    /// </summary>
    public abstract void Disable();
}