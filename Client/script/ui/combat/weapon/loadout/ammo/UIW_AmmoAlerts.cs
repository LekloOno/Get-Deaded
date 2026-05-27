using System;
using Godot;
using Godot.Collections;

public partial class UIW_AmmoAlerts : Control
{
    [Export] private PW_WeaponsHandler? _weaponsHandler;
    [Export] private UIW_LowUnloaded? _lowUnloaded;
    [Export] private UIW_LowLoaded? _lowLoaded;

    private bool _reloading;
    
    private PW_Ammunition? _currentAmmos;
    private uint _lowLoadedThreshold;
    private uint _lowUnloadedThreshold;
    private uint _criticalLowUnloadedThreshold;

    public override void _Ready()
    {
        if (_weaponsHandler == null)
        {
            GD.PushError("[UIW_AmmoAlerts] Missing PW_WeaponsHandler reference.");
            return;
        }

        _lowLoaded?.StartHide();
        _lowUnloaded?.Hide();

        _weaponsHandler.GotInitialized += OnInitialized;
        _weaponsHandler.SwitchEnded += OnSwitchEnded;
        _weaponsHandler.ReloadStarted += OnReloadStarted;
        _weaponsHandler.ReloadCancelled += OnReloadCanceled;
        _weaponsHandler.ReloadReady += OnReloadCanceled;
    }

    private void OnReloadCanceled()
    {
        _reloading = false;
        Update();
    }

    private void OnReloadStarted(float obj)
    {
        _reloading = true;
        _lowLoaded?.StartHide();
    }

    private void OnSwitchEnded(object? sender, PW_Weapon e)
    {
        BindWeapon(e);
    }

    private void OnInitialized(PW_Weapon active, PW_Weapon nextHolster, int nextIndex, Array<PW_Weapon> weapons)
    {
        BindWeapon(active);
    }
    
    private void BindWeapon(PW_Weapon e)
    {
        if (_currentAmmos != null)
        {
            // Not compatible with alternate fire .. we'll figure it out later
            // The weapon system will go through a rework anyways
            _currentAmmos.LoadedChanged -= OnTotalChanged;
            _currentAmmos.UnloadedChanged -= OnTotalChanged;
        }

        _currentAmmos = e.Fires[0].Ammos;
        
        uint magPick = _currentAmmos.MagazinePick;

        if (magPick == 0)
        {
            _lowLoaded?.StartHide();
            _lowUnloaded?.Hide();
            return;
        }

        _currentAmmos.LoadedChanged += OnTotalChanged;
        _lowLoadedThreshold = (magPick * 10 + 35) / 36;

        _currentAmmos.UnloadedChanged += OnTotalChanged;
        _lowUnloadedThreshold = magPick * 3;
        _criticalLowUnloadedThreshold = magPick;

        Update();
    }

    private void Update()
    {
        if (_currentAmmos == null)
            return;

        OnTotalChanged(0, 0);
    }

    private void OnTotalChanged(int amount, uint finalAmount)
    {
        OnUnloadedChanged();
        OnLoadedChanged();
    }

    private void OnUnloadedChanged()
    {
        if (_currentAmmos == null)
            return;

        if (_lowUnloaded == null)
            return;

        uint totalAmmos = _currentAmmos.TotalAmmos();

        if (totalAmmos > _lowUnloadedThreshold)
        {
            _lowUnloaded.Hide();
            return;
        }

        _lowUnloaded.Show();

        if (totalAmmos == 0)
            _lowUnloaded.SetEmpty();
        else if (totalAmmos <= _criticalLowUnloadedThreshold)
            _lowUnloaded.SetCritical();
        else
            _lowUnloaded.SetNormal();
    }

    private void OnLoadedChanged()
    {
        if (_currentAmmos == null)
            return;

        if (_lowLoaded == null)
            return;

        if (_reloading)
            return;

        if (_currentAmmos.LoadedAmmos <= _lowLoadedThreshold && _currentAmmos?.UnloadedAmmos != 0)
            _lowLoaded.StartShow();
        else
            _lowLoaded.StartHide();
    }
}