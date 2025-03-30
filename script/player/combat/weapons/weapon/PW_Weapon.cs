using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public abstract partial class PW_Weapon : Resource
{
    [Export] public float SwitchInTime {get; private set;}
    [Export] public float SwitchOutTime {get; private set;}
    [Export] public float MoveSpeedModifier {get; private set;} = 0f;   // An additive modifier to set. - is a malus + is a bonus
    [Export] public float ReloadTime {get; private set;}
    [Export] public float TacticalReloadTime {get; private set;}
    [Export] public float ReloadReadyTime {get; private set;}            // Additionnal time before the weapon is ready once it's reloaded, allow annimation cancels
    [Export] protected PW_ADS _ads;
    [Export] public Texture2D Icon {get; private set;}
    [Export] public Color IconColor {get; private set;}
    protected PC_DirectCamera _camera;
    protected Node3D _sight;
    protected Node3D _barel;
    private PM_SurfaceControl _surfaceControl;
    public EventHandler<ShotHitEventArgs> Hit;
    public Action ADSStarted;
    public Action ADSStopped;
    public bool ADSActive {get; private set;} = false;          // Not ideal, maybe make so ads return 3 state instead of true/false to know when nothing happened.

    public void Initialize(PC_DirectCamera camera, Node3D sight, Node3D barel, PM_SurfaceControl surfaceControl, PC_Recoil recoilController)
    {
        _camera = camera;
        _sight = sight;
        _barel = barel;
        _surfaceControl = surfaceControl;
        _ads?.Initialize(_camera);
        WeaponInitialize(recoilController);
    }   

    public void HandleSecondaryDown()
    {
        if (_ads == null)
        {
            SecondaryDown();
            return;
        }

        if (_ads.Pressed())
            ActivateADS();
        else
            DeactivateADS();
    }

    public void HandleSecondaryUp()
    {
        if (_ads == null)
        {
            SecondaryUp();
            return;
        }

        if (_ads.Released())
            DeactivateADS();
    }

    public void HandlePrimaryDown() => PrimaryDown();   // For naming consistency
    public void HandlePrimaryUp() => PrimaryUp();       // For naming consistency
    public void HandleReload() => Reload();             // For naming consistency
    public bool HandleCanReload(out float reloadTime)
    {
        bool canReload = CanReload(out bool tactical);
        reloadTime = tactical ? TacticalReloadTime : ReloadTime;
        return canReload;
    }

    protected abstract bool CanReload(out bool tactical);

    public void HandleDisable()
    {
        if (_ads != null && _ads.Disable())
        {
            DeactivateADS();
            _surfaceControl.SpeedModifiers.Remove(_ads.MoveSpeedMultiplier);
        }

        Disable();
    }
    
    /// <summary>
    /// Allows to retrieve existing fire modes for this weapon.
    /// </summary>
    /// <returns></returns>
    public abstract List<PW_Fire> GetFireModes();

    /// <summary>
    /// Allow for some specific initialization.
    /// </summary>
    protected virtual void WeaponInitialize(PC_Recoil recoilController) {}

    /// <summary>
    /// Called when the reload input is pressed down.
    /// </summary>
    protected abstract void Reload();
    /// <summary>
    /// Called when the primary input is pressed down. Will typically handle shooting process.
    /// </summary>
    protected abstract void PrimaryDown();
    /// <summary>
    /// Called when the primary input is released up. Could handle some special behaviors, or shooting.
    /// </summary>
    protected abstract void PrimaryUp();

    /// <summary>
    /// Called if the ADS handler didn't consume the incoming secondary input pressed down.
    /// </summary>
    protected abstract void SecondaryDown();
    /// <summary>
    /// Called if the ADS handler didn't consume the incoming secondary input pressed up.
    /// </summary>
    protected abstract void SecondaryUp();
    /// <summary>
    /// Called right after handling a starting ADS input.
    /// </summary>
    protected abstract void StartADS();
    /// <summary>
    /// Called right after handling a stopping ADS input.
    /// </summary>
    protected abstract void StopADS();

    /// <summary>
    /// Allow for some specific disabling process.
    /// </summary>
    protected abstract void Disable();

    public abstract bool PickAmmo(int amount, bool magazine, int targetFireIndex);

    private void ActivateADS()
    {
        if (ADSActive)
                return;

        ADSActive = true;
        _surfaceControl.SpeedModifiers.Add(_ads.MoveSpeedMultiplier);
        StartADS();
        ADSStarted?.Invoke();
    }

    public void DeactivateADS()
    {
        if (!ADSActive)
                return;
            
        ADSActive = false;
        _surfaceControl.SpeedModifiers.Remove(_ads.MoveSpeedMultiplier);
        StopADS();
        ADSStopped?.Invoke();
    }

}