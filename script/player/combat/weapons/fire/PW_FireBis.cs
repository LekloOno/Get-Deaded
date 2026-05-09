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
public abstract partial class PW_FireBis : WeaponComponent
{
    [Export] protected Array<PW_ShotBis> _shots;
    [Export] protected ulong _fireRate;
    [Export] protected PW_Ammunition _ammos;
    [Export] protected uint _ammosPerShot = 1;
    protected bool _enabled = true;

    public void AddDamageMultiplier(float multiplier)
    {
        foreach (PW_ShotBis shot in _shots)
            shot.DamageMultiplier.Add(multiplier);
    }

    public void RemoveDamageMultiplier(float multiplier)
    {
        foreach (PW_ShotBis shot in _shots)
            shot.DamageMultiplier.Remove(multiplier);        
    }

    public bool InfiniteAmmo
    {
        get => _ammos.InfiniteAmmo;
        set => _ammos.InfiniteAmmo = value;
    }

    [Export] public bool InfiniteMagazine = false;
    public PW_Ammunition Ammos => _ammos;
    public Action<HitEventArgs> Hit;
    public Action<int> Shot;      // Event arg is the amount of shots shot, most likely _shots.Count


    protected ulong _lastShot = 0;
    private static Random _random = new();


    public void Initialize(GE_IActiveCombatEntity owner)
    {
        
        _ammos.Initialize();

        SpecInitialize(owner);

        foreach (PW_ShotBis shot in _shots)
        {
            shot.Initialize(owner);
            shot.Hit += (e) => Hit?.Invoke(e);
        }
    }

    public bool Press() => SpecPress();
    public bool Release() => SpecRelease();

    public ulong NextAvailableShot() => _fireRate + _lastShot - PHX_Time.ScaledTicksMsec;

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
        _lastShot = PHX_Time.ScaledTicksMsec;
        
        foreach (PW_ShotBis shot in _shots)
            shot.Shoot();
        
        Shot?.Invoke(_shots.Count);
    }

    protected bool CanShoot() => _enabled
        && PHX_Time.ScaledTicksMsec - _lastShot >= _fireRate
        && (InfiniteMagazine || _ammos.DidConsume(_ammosPerShot));

    public void Reload()
    {
        _ammos.Reload();
    }

    public bool CanReload(out bool tactical)
    {
        tactical = _ammos.LoadedAmmos > 0;
        return _ammos.CanReload();
    }

    public void AddSpread(float multiplier)
    {
        foreach (PW_ShotBis shot in _shots)
            shot.SpreadMultiplier?.Add(multiplier);
    }

    public void RemoveSpread(float multiplier)
    {
        foreach (PW_ShotBis shot in _shots)
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
    protected abstract void SpecInitialize(GE_IActiveCombatEntity owner);
    public void Enable()
    {
        _enabled = true;
    }
    public void Disable()
    {
        _enabled = false;
        DisableSpec();
    }
    /// <summary>
    /// Handle the weapon disabling. Example - A weapons which shoots continuously should stop shooting on disable, even if no up input has been sent.
    /// </summary>
    protected abstract void DisableSpec();

    public void Sleep()
    {
        foreach (PW_ShotBis shot in _shots)
            shot.Sleep();
    }

    public void Awake()
    {
        foreach (PW_ShotBis shot in _shots)
            shot.Awake();
    }
}