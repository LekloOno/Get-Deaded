using System;
using System.Linq;
using Godot;
using Godot.Collections;

public delegate void SwitchEvent(PW_Weapon active, PW_Weapon nextHolster, int nextIndex, Array<PW_Weapon> weapons);

[GlobalClass, Icon("res://gd_icons/weapon_system/WeaponSystemIcon")]
public partial class PW_WeaponsHandler : WeaponSystem
{
    [Export] private PC_DirectCamera _camera;
    [Export] private PC_Shakeable _shakeableCamera;
    [Export] private PC_Recoil _recoilController;
    [Export] private Node3D _sight;
    [Export] private Node3D _barel;
    [Export] private GB_ExternalBodyManager _ownerBody;
    [Export] private PI_Weapons _weaponsInput;
    [Export] private Array<PW_Weapon> _weapons;
    [Export] private PW_Weapon _melee;
    [Export] private PM_SurfaceControl _surfaceControl;
    [Export] private float _meleeRecover;
    public EventHandler<ShotHitEventArgs> Hit;

    public SwitchEvent SwitchStarted;
    public SwitchEvent Initialized;
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
    public bool ADSactive => _activeWeapon.ADSActive;
    public Action ReloadReady;
    public Action Reloaded;
    public Action ADSStarted;
    public Action ADSStopped;
    private Action Available;

    private PW_Weapon _activeWeapon;
    private int _weaponIndex = 0;
    private PW_Weapon _targetWeapon;      // The weapon we are currently switching to, if it's _activeWeapon, no switch is happening
    private SceneTreeTimer _switchTimer;
    private bool _switchingOut = false;
    private bool _switchingIn = false;
    private bool _bufferedPrimary = false;
    private bool _bufferedSecondary = false;

    private bool _reloading = false;
    private bool _ready = true;

    private SceneTreeTimer _reloadTimer;
    private SceneTreeTimer _meleeRecoverTimer;
    

    public override void _Ready()
    {
        foreach(PW_Weapon weapon in _weapons)
        {
            weapon.Initialize(_shakeableCamera, _camera, _sight, _surfaceControl, _recoilController, _ownerBody);
            weapon.Hit += (o, e) => Hit?.Invoke(o, e);
            weapon.ADSStarted += () => ADSStarted?.Invoke();
            weapon.ADSStopped += () => ADSStopped?.Invoke();
        }

        _melee.Initialize(_shakeableCamera, _camera, _sight, _surfaceControl, _recoilController, _ownerBody);
        _melee.Hit += (o, e) => Hit?.Invoke(o, e);

        _activeWeapon = _melee;
        _targetWeapon = _melee;
        _surfaceControl.SpeedModifiers.Add(_activeWeapon.MoveSpeedModifier);

        _weaponsInput.OnStartPrimary += HandleStartPrimary;
        _weaponsInput.OnStopPrimary += HandleStopPrimary;
        _weaponsInput.OnStartSecondary += HandleStartSecondary;
        _weaponsInput.OnStopSecondary += HandleStopSecondary;
        _weaponsInput.OnSwitch += Switch;
        _weaponsInput.OnHolster += Holster;
        _weaponsInput.OnReload += Reload;
        _weaponsInput.OnStartMelee += DirectMeleeStart;
        _weaponsInput.OnStopMelee += DirectMeleeStop;

        _melee.Shot += OnMeleeShot;

        SwitchEnded += (o, e) => Available?.Invoke();
        ReloadReady += () => Available?.Invoke();

        int nextIndex = (_weaponIndex + 1) % _weapons.Count;
        Initialized?.Invoke(_melee, _weapons[_weaponIndex], nextIndex, _weapons);
    }

    private void DirectMeleeStop(object sender, EventArgs e) => _melee.PrimaryRelease();
    private void DirectMeleeStart(object sender, EventArgs e) => _melee.PrimaryPress();

    private void OnMeleeShot()
    {
        if (_activeWeapon == _melee)
            return;
        
        CancelReload();
        _activeWeapon.Disable();
        
        _ready = false;

        if (_switchingIn)
            EndSwitch();

        if (_meleeRecoverTimer != null)
            _meleeRecoverTimer.Timeout -= EndMeleeRecover;

        _meleeRecoverTimer = GetTree().CreateTimer(_meleeRecover);
        _meleeRecoverTimer.Timeout += EndMeleeRecover;
    }

    private void EndMeleeRecover()
    {
        _ready = true;
        Available?.Invoke();
    }

    public void InitData(out PW_Weapon active, out PW_Weapon nextHolster, out int nextIndex, out Array<PW_Weapon> weapons)
    {
        active = _activeWeapon;
        nextHolster = _activeWeapon == _melee ? _weapons[_weaponIndex] : _melee;
        nextIndex = (_weaponIndex + 1) % _weapons.Count;
        weapons = _weapons;
    }

    public bool ActiveWeaponHalted() => !_ready || _switchingIn || _switchingOut;
    public bool IsSwitching() => _switchingIn || _switchingOut;

    // Very redondant way to define the buffers, could use a little rework !
    private void BufferPrimary()
    {
        if (_bufferedPrimary)
            return;

        _bufferedPrimary = true;
        Available += SendPrimary;
    }

    private void ResetPrimaryBuffer()
    {
        if (!_bufferedPrimary)
            return;

        _bufferedPrimary = false;
        Available -= SendPrimary;
    }

    private void SendPrimary()
    {
        ResetPrimaryBuffer();
        _activeWeapon.PrimaryPress();
    }

    private void HandleStartPrimary(object sender, EventArgs e)
    {
        _melee.ResetBuffer();
        if (ActiveWeaponHalted())
        {
            BufferPrimary();
            return;
        }

        if (_bufferedPrimary)
            ResetPrimaryBuffer();

        _activeWeapon?.PrimaryPress();
    }
    private void HandleStopPrimary(object sender, EventArgs e)
    {
        if (_bufferedPrimary)
            ResetPrimaryBuffer();
        
        _activeWeapon?.PrimaryRelease();
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
        _activeWeapon.SecondaryPress();
    }
    private void HandleStartSecondary(object sender, EventArgs e)
    {
        if (ActiveWeaponHalted())
        {
            if (_bufferedSecondary)
                return;

            BufferSecondary();
            return;
        }

        if (_bufferedSecondary)
            ResetSecondaryBuffer();


        _activeWeapon?.SecondaryPress();
    }
    private void HandleStopSecondary(object sender, EventArgs e)
    {
        if (_bufferedSecondary)
            ResetSecondaryBuffer();

        _activeWeapon?.SecondaryRelease();
    }


    public void Switch(object sender, EventArgs e)
    {
        _weaponIndex = (_weaponIndex + 1) % _weapons.Count;
        _targetWeapon = _weapons[_weaponIndex];

        SwitchStarted?.Invoke(_targetWeapon, _melee, (_weaponIndex + 1) % _weapons.Count, _weapons);
        StartSwitch();
    }

    public void Holster(object sender, EventArgs e)
    {
        PW_Weapon targetPrev = _targetWeapon;
        if (_targetWeapon == _melee)
            _targetWeapon = _weapons[_weaponIndex];
        else
            _targetWeapon = _melee;

        SwitchStarted?.Invoke(_targetWeapon, targetPrev, (_weaponIndex + 1) % _weapons.Count, _weapons);
        StartSwitch();
    }

    public void Reload(object sender, EventArgs e)
    {
        if(!_ready || _reloading || IsSwitching())
            return;

        if(!_activeWeapon.CanReload(out float reloadTime))
            return;

        _reloading = true;
        _ready = false;
        _reloadTimer = GetTree().CreateTimer(reloadTime);
        _reloadTimer.Timeout += DoReload;
    }

    private void DoReload()
    {
        _activeWeapon.Reload();
        _reloadTimer = GetTree().CreateTimer(_activeWeapon.ReloadReadyTime);
        _reloadTimer.Timeout += CompleteReload;
        _reloading = false;
    }

    private void CompleteReload()
    {
        _ready = true;
        ReloadReady?.Invoke();
    }

    private void CancelReload()
    {
        if (_reloadTimer == null)
            return;
        
        if (_reloading)
            _reloadTimer.Timeout -= DoReload;
        else
            _reloadTimer.Timeout -= CompleteReload;

        _reloadTimer = null;
        _reloading = false;
        _ready = true;
    }

    public void StartSwitch()
    {
        CancelReload();

        // Could be done with index instead, but having only one weapon and holster could cause bug if so
        //   - Cancel will always happen, since the index will always be 0
        if (_targetWeapon == _activeWeapon)   // The player is switching back to its initial weapon
        {                                   // We can cancel the switch.
            EndSwitch();
            SwitchCanceled?.Invoke(this, _activeWeapon);
            return;
        }

        if (_switchingOut)  // The player is switching out his initial weapon
            return;         // We can just change the target weapon (nextWeapon) with no penalty, the new target switch in time will be used.   

        if (_switchingIn)   // The player is switching in his target weapon
        {                   // We want to restart the switch in process. Otherwize he could abuse the short switch in time of, typically, holster, to switch to other weapons.
            float time = _targetWeapon.SwitchOutTime; // (Holster, then switch weapon : the holster switch in time will be used, but the target weapon will be the side weapon)
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
        _activeWeapon.Disable();

        _switchTimer = GetTree().CreateTimer(time);
        _switchTimer.Timeout += OnSwitchIn;
        
        SwitchOut?.Invoke(this, _activeWeapon);
    }

    public void OnSwitchIn()
    {
        _switchingOut = false;
        _switchingIn = true;

        float time = _targetWeapon.SwitchOutTime;

        _switchTimer = GetTree().CreateTimer(time);
        _switchTimer.Timeout += EndSwitch;

        SwitchIn?.Invoke(this, _targetWeapon);
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

        _surfaceControl.SpeedModifiers.Remove(_activeWeapon.MoveSpeedModifier);
        _surfaceControl.SpeedModifiers.Add(_targetWeapon.MoveSpeedModifier);
        _activeWeapon = _targetWeapon;

        SwitchEnded?.Invoke(this, _activeWeapon);
    }

    public bool PickAmmo(GL_AmmoData data)
    {
        // Distribute ammos among all weapons
        if (data.WeaponIndex == 0)
        {
            int count = _weapons.Count;
            int distribAmount = data.Amount / count;
            int remainder = data.Amount % count;

            bool couldPick = false;

            for (int i = 0; i < count; i++)
            {
                int realAmount = distribAmount + (i < remainder ? 1 : 0);
                couldPick |= _weapons.ElementAt(i).PickAmmo(realAmount, data.Magazine, data.FireIndex);
            }

            return couldPick;
        }

        // Distribute ammos to the current weapon
        if (data.WeaponIndex == -1)
            return _activeWeapon.PickAmmo(data.Amount, data.Magazine, data.FireIndex);

        // Distribute ammos to the target weapon
        int realIndex = data.WeaponIndex - 1;
        return _weapons.ElementAt(realIndex % _weapons.Count).PickAmmo(data.Amount, data.Magazine, data.FireIndex);
    }
}