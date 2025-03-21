//using System.Linq;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class PW_WeaponsHandler : Node
{
    [Export] private PC_DirectCamera _camera;
    [Export] private Node3D _sight;
    [Export] private Node3D _barel;
    [Export] private PI_Weapons _weaponsInput;
    [Export] private Array<PW_Weapon> _weapons;
    [Export] private PW_Weapon _melee;
    [Export] private PM_SurfaceControl _surfaceControl;
    public EventHandler<ShotHitEventArgs> Hit;

    /// <summary>
    /// Event Arg is the weapon the player is switching out from.
    /// </summary>
    public EventHandler<PW_Weapon> SwitchOut;
    /// <summary>
    /// Event Arg is the weapon the player is switching in to.
    /// </summary>
    public EventHandler<PW_Weapon> SwitchIn;
    /// <summary>
    /// Event Arg is the weapon the player canceled back to.
    /// </summary>
    public EventHandler<PW_Weapon> SwitchCanceled;
    /// <summary>
    /// Event Arg is the weapon the player has now active.
    /// </summary>
    public EventHandler<PW_Weapon> SwitchEnded;

    private PW_Weapon _activeWeapon;
    private int _weaponIndex = 0;
    private PW_Weapon _nextWeapon;      // The weapon we are currently switching to, if it's _activeWeapon, no switch is happening
    private PW_Weapon _prevWeapon;
    private SceneTreeTimer _switchTimer;
    private bool _switchingOut = false;
    private bool _switchingIn = false;
    private bool _bufferedPrimary = false;
    private bool _bufferedSecondary = false;

    public override void _Ready()
    {
        foreach(PW_Weapon weapon in _weapons)
        {
            weapon.Initialize(_camera, _sight, _barel, _surfaceControl);
            weapon.Hit += (o, e) => Hit?.Invoke(o, e);
        }

        _melee.Initialize(_camera, _sight, _barel, _surfaceControl);
        _melee.Hit += (o, e) => Hit?.Invoke(o, e);

        _activeWeapon = _melee;
        _surfaceControl.SpeedModifiers.Add(_activeWeapon.MoveSpeedModifier);

        _weaponsInput.OnStartPrimary += HandleStartPrimary;
        _weaponsInput.OnStopPrimary += HandleStopPrimary;
        _weaponsInput.OnStartSecondary += HandleStartSecondary;
        _weaponsInput.OnStopSecondary += HandleStopSecondary;
        _weaponsInput.OnSwitch += Switch;
        _weaponsInput.OnHolster += Holster;
    }

    // Very redondant way to define the buffers, could use a little rework !
    private void BufferPrimary()
    {
        _bufferedPrimary = true;
        SwitchEnded += SendPrimary;
    }
    private void ResetPrimaryBuffer()
    {
        _bufferedPrimary = false;
        SwitchEnded -= SendPrimary;
    }
    private void SendPrimary(object sender, PW_Weapon e)
    {
        ResetPrimaryBuffer();
        _activeWeapon.HandlePrimaryDown();
    }
    private void HandleStartPrimary(object sender, EventArgs e)
    {
        if (_activeWeapon == null)
        {
            if (_bufferedPrimary)
                return;

            BufferPrimary();
            return;
        }

        if (_bufferedPrimary)
            ResetPrimaryBuffer();

        _activeWeapon?.HandlePrimaryDown();
    }
    private void HandleStopPrimary(object sender, EventArgs e)
    {
        if (_bufferedPrimary)
            ResetPrimaryBuffer();
        
        _activeWeapon?.HandlePrimaryUp();
    }

    private void BufferSecondary()
    {
        _bufferedSecondary = true;
        SwitchEnded += SendSecondary;
    }
    private void ResetSecondaryBuffer()
    {
        _bufferedSecondary = false;
        SwitchEnded -= SendSecondary;
    }
    private void SendSecondary(object sender, PW_Weapon e)
    {
        ResetSecondaryBuffer();
        _activeWeapon.HandleSecondaryDown();
    }
    private void HandleStartSecondary(object sender, EventArgs e)
    {
        if (_activeWeapon == null)
        {
            if (_bufferedSecondary)
                return;

            BufferSecondary();
            return;
        }

        if (_bufferedSecondary)
            ResetSecondaryBuffer();


        _activeWeapon?.HandleSecondaryDown();
    }
    private void HandleStopSecondary(object sender, EventArgs e)
    {
        if (_bufferedSecondary)
            ResetSecondaryBuffer();

        _activeWeapon?.HandleSecondaryUp();
    }


    public void Switch(object sender, EventArgs e)
    {
        _weaponIndex = (_weaponIndex + 1) % _weapons.Count;
        _nextWeapon = _weapons[_weaponIndex];

        StartSwitch();
    }

    public void Holster(object sender, EventArgs e)
    {
        if (_activeWeapon == _melee)
            _nextWeapon = _weapons[_weaponIndex];
        else
            _nextWeapon = _melee;

        StartSwitch();
    }

    public void StartSwitch()
    {
        // Could be done with index instead, but having only one weapon and holster could cause bug if so
        //   - Cancel will always happen, since the index will always be 0
        if (_nextWeapon == _activeWeapon)   // The player is switching back to its initial weapon
        {                                   // We can cancel the switch.
            EndSwitch();
            SwitchCanceled?.Invoke(this, _activeWeapon);
            return;
        }

        if (_switchingOut)  // The player is switching out his initial weapon
            return;         // We can just change the target weapon (nextWeapon) with no penalty, the new target switch in time will be used.   

        if (_switchingIn)   // The player is switching in his target weapon
        {                   // We want to restart the switch in process. Otherwize he could abuse the short switch in time of, typically, holster, to switch to other weapons.
            float time = _nextWeapon.SwitchOutTime; // (Holster, then switch weapon : the holster switch in time will be used, but the target weapon will be the side weapon)
            _switchTimer.Timeout -= EndSwitch;
            _switchTimer = GetTree().CreateTimer(time);
            _switchTimer.Timeout += EndSwitch;
            return;
        }

        OnSwitchOut();
    }

    public void OnSwitchOut()
    {
        _switchingIn = false;
        _switchingOut = true;

        float time = _activeWeapon.SwitchOutTime;
        _activeWeapon.HandleDisable();
        _prevWeapon = _activeWeapon;
        _activeWeapon = null;

        _switchTimer = GetTree().CreateTimer(time);
        _switchTimer.Timeout += OnSwitchIn;
        
        SwitchOut?.Invoke(this, _prevWeapon);
    }

    public void OnSwitchIn()
    {
        _switchingOut = false;
        _switchingIn = true;

        float time = _nextWeapon.SwitchOutTime;

        _switchTimer = GetTree().CreateTimer(time);
        _switchTimer.Timeout += EndSwitch;

        SwitchIn?.Invoke(this, _nextWeapon);
    }

    public void EndSwitch()
    {
        if (_switchTimer != null)
        {
            if (_switchingOut)
            {
                _switchingOut = false;
                _switchTimer.Timeout -= OnSwitchIn;
            }
            else if (_switchingIn)
            {
                _switchingIn = false;
                _switchTimer.Timeout -= EndSwitch;
            }
        }

        _surfaceControl.SpeedModifiers.Remove(_prevWeapon.MoveSpeedModifier);
        _surfaceControl.SpeedModifiers.Add(_nextWeapon.MoveSpeedModifier);
        _activeWeapon = _nextWeapon;

        SwitchEnded?.Invoke(this, _activeWeapon);
    }
}