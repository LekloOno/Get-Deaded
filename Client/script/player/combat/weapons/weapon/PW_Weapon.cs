
using System;
using Godot;
using Godot.Collections;

/// <summary>
/// Base Weapon Component.
/// <para>
/// You can inherits from this class to create new weapon specifications.
/// </para>
/// </summary>

/*
--------------------------------------------------------------------
Logic elements
- A generic list of PW_Fire.
- (Optional) A PW_ADS.
- A move speed modifier to apply when this weapon is held.
- Reload Informations - time, tactical time and ready time
- Switching informations - Switch in and out.

--------------------------------------------------------------------
And Visual elements
- Icon & Icon Color.
--------------------------------------------------------------------
Connections
- Hit - an Event casted with a ShotHitEventArgs when this weapon has Hit.
- Shot - an Action casted when this weapon has Shot.
- ADSStarted - an Event casted when this weapon entered ADS mode.
- ADSStarted - an Event casted when this weapon exited ADS mode.
*/

// Icon credits - Skoll - under CC BY 3.0 - https://game-icons.net/1x1/skoll/ak47u.html
[GlobalClass, Icon("res://gd_icons/weapon_system/weapon_icon.svg")]
public abstract partial class PW_Weapon : WeaponComponent
{
    [Export] public DATA_Weapon Data {get; private set;}
    [Export] public PW_WeaponReloaderData ReloaderData {get; private set;}
    public PW_WeaponReloader Reloader { get; private set; }
    [Export] public float MoveSpeedModifier {get; private set;} = 0f;   // An additive modifier to set. - is a malus + is a bonus
    [Export] public float SwitchInTime {get; private set;}
    [Export] public float SwitchOutTime {get; private set;}
    [Export] protected Array<PW_Fire> _fires;
    public Array<PW_Fire> Fires => _fires;
    [Export] protected PW_ADS _ads;
    private PM_SurfaceControl _surfaceControl;

    [ExportCategory("Visuals")]
    public Texture2D Icon => Data.Icon;
    public Color IconColor => Data.IconColor;
    public PW_WeaponsHandler Handler {get; private set;}
    
    public bool ADSActive => _ads == null ? false : _ads.Active;
    public EventHandler<HitEventArgs> Hit;
    public Action Shot;
    public Action ADSStarted;
    public Action ADSStopped;
    
    protected PW_Fire _currentFire;

    /// -----------------------------
    ///      ___                
    ///     | _ ) __ _  ___ ___ 
    ///     | _ \/ _` |(_-</ -_)
    ///     |___/\__,_|/__/\___|
    /// 
    /// -----------------------------
    /// Base Definitions for weapons.

    #region base
    /// <summary>
    /// Initialize the external references for the weapon and propagate initialization to its children components.
    /// </summary>
    /// <param name="shakeableCamera">A shakeable node for camera shakes.</param>
    /// <param name="camera">The owner's camera.</param>
    /// <param name="surfaceControl">The owner's surface control.</param>
    /// <param name="recoilController">The owner's recoil controller.</param>
    /// <param name="ownerBody">The owner's external forces manager.</param>
    public void Initialize(PC_Shakeable shakeableCamera, PC_DirectCamera camera, PM_SurfaceControl surfaceControl, PC_Recoil recoilController, GB_ExternalBodyManagerWrapper ownerBody, PW_WeaponsHandler handler)
    {
        Handler = handler;
        _surfaceControl = surfaceControl;
        if (_ads != null)
        {
            _ads.Initialize(camera, surfaceControl);
            _ads.Started += StartADS;
            _ads.Stopped += StopADS;
        }

        SpecInitialize(shakeableCamera, recoilController, ownerBody);

        foreach(PW_Fire fire in _fires)
        {
            fire.Initialize(shakeableCamera, recoilController, ownerBody, this);
            fire.Shot += (o, e) => Shot?.Invoke();
            fire.Hit += (o, e) => Hit?.Invoke(o, e);
        }
        _currentFire = InitCurrentFire();
    }

    public override void _Ready()
    {
        Reloader = new(ReloaderData);
        AddChild(Reloader);

        Reloader.Unloaded += Unload;
        Reloader.Inserted += Insert;
        Reloader.Recovered += SpecEnable;
        Reloader.Chambered += Chamber;

        foreach (PW_Fire fire in _fires)
            fire.DryShot += DryTryReload;
    }

    private void DryTryReload() => TryReload();

    public virtual void Sleep() =>
        _currentFire.Sleep();

    public virtual void Awake() =>
        _currentFire.Awake();

    public void SetInfiniteAmmo(bool active)
    {
        foreach (PW_Fire fire in _fires)
            fire.InfiniteAmmo = active;
    }

    public void InitializeAmmos()
    {
        foreach (PW_Fire fire in Fires)
            fire.Ammos.Initialize();

        Reloader.Reset();
    }

    public void SetInfiniteMagazine(bool active)
    {
        foreach (PW_Fire fire in _fires)
            fire.InfiniteMagazine = active;
    }

    public void Disable()
    {
        _ads?.Disable();
        SpecDisable();

        if (!Reloader.IsReady)
            TryCancelReload();
    }

    public void Enable()
    {
        SpecEnable();

        if (!Reloader.IsReady)
            TryReload();
    }

    public void AddDamageMultiplier(float multiplier)
    {
        foreach (PW_Fire fire in _fires)
            fire.AddDamageMultiplier(multiplier);
    }

    public void RemoveDamageMultiplier(float multiplier)
    {
        foreach (PW_Fire fire in _fires)
            fire.RemoveDamageMultiplier(multiplier);
    }

    public void ClearDamageMultiplier()
    {
        foreach (PW_Fire fire in _fires)
            fire.ClearDamageMultiplier();
    }
    
    /// <summary>
    /// Allows to retrieve existing fire modes for this weapon.
    /// </summary>
    /// <returns></returns>
    public Array<PW_Fire> GetFireModes() => _fires;

    public void ResetBuffer()
    {
        foreach (PW_Fire fire in _fires)
            fire.ResetBuffer();
    }

    public bool SecondaryPress()
    {
        if (_ads != null)
            return _ads.Press();
        
        if (!Reloader.IsReady)
            return false;
        
        return SpecSecondaryPress();
    }

    public bool SecondaryRelease()
    {
        if (_ads != null)
            return _ads.Release();
        
        if (!Reloader.IsReady)
            return false;

        else
            return SpecSecondaryRelease();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="chambered">whether it is needed to chamber the bullet or not.</param>
    /// <returns></returns>
    public abstract bool CanReload(out bool chambered);

    private void StartADS()
    {
        SpecStartADS();
        ADSStarted?.Invoke();
    }
    private void StopADS()
    {
        SpecStopADS();
        ADSStopped?.Invoke();
    }

    public bool TryCancelReload() =>
        Reloader.Cancel();

    public bool TryReload()
    {
        if (!CanReload(out bool chambered))
            return false;

        Interrupt();
        return Reloader.StartReload(chambered);
    }

    #endregion

    /// ----------------------------------------------------------------
    ///  ___                 _        _  _            _    _            
    /// / __| _ __  ___  __ (_) __ _ | |(_) ___ __ _ | |_ (_) ___  _ _  
    /// \__ \| '_ \/ -_)/ _|| |/ _` || || ||_ // _` ||  _|| |/ _ \| ' \ 
    /// |___/| .__/\___|\__||_|\__,_||_||_|/__|\__,_| \__||_|\___/|_||_|
    ///      |_|                                                        
    /// ----------------------------------------------------------------
    /// Signatures for specialized weapons.
    
    #region abstract
    public abstract bool PickAmmo(int amount, bool magazine, int targetFireIndex);

    /// <summary>
    /// Allow for some specific initialization.
    /// </summary>
    protected abstract void SpecInitialize(PC_Shakeable shakeableCamera, PC_Recoil recoilController, GB_ExternalBodyManagerWrapper ownerBody);
    protected virtual PW_Fire InitCurrentFire() => _fires[0];
    /// <summary>
    /// Allow for some specific disabling process.
    /// </summary>
    protected virtual void SpecDisable() => _currentFire.Disable();
    /// <summary>
    /// Allow for some specific disabling process.
    /// </summary>
    protected virtual void SpecEnable() => _currentFire.Enable();
    

    /// <summary>
    /// Called when the primary input is pressed down. Will typically handle shooting process.
    /// </summary>
    public bool PrimaryPress()
    {
        if (Reloader.IsReady)
            return _currentFire.Press();
        else
            return false;
    } 
    /// <summary>
    /// Called when the primary input is released up. Could handle some special behaviors, or shooting.
    /// </summary>
    public bool PrimaryRelease()
    {
        if (Reloader.IsReady)
            return _currentFire.Release();
        else
            return false;   
    }
    /// <summary>
    /// Called if the ADS handler didn't consume the incoming secondary press input.
    /// </summary>
    protected abstract bool SpecSecondaryPress();
    /// <summary>
    /// Called if the ADS handler didn't consume the incoming secondary release input.
    /// </summary>
    protected abstract bool SpecSecondaryRelease();


    /// <summary>
    /// Called right after handling a starting ADS input.
    /// </summary> 
    protected abstract void SpecStartADS();
    /// <summary>
    /// Called right after handling a stopping ADS input.
    /// </summary>
    protected abstract void SpecStopADS();

    public void Unload()
    {
        foreach (PW_Fire fire in _fires)
            fire.Unload();
    }

    public void Insert()
    {
        foreach (PW_Fire fire in _fires)
            fire.Insert();
    }

    public void Chamber()
    {
        foreach (PW_Fire fire in _fires)
            fire.Chamber();
    }

    public virtual void Interrupt()
    {
        foreach (PW_Fire fire in _fires)
            fire.Disable();
    }

    #endregion
}