using System.Collections.Generic;
using Godot;

public partial class UI_CrosshairPresets : Node
{
    [Export] private UI_EscapeMenu      _menu   = null!;
    [Export] private UI_CrosshairGalery _galery = null!;
    [Export] private Button? _presetsButton;
    [Export] private Button? _savedButton;

    const string CrosshairPresetsJsonRegistry = "res://config/assets/crosshair/registry.json";
    const string CrosshairSavedDir = "user://crosshair/saved";

    public override void _Ready()
    {
        if (_presetsButton != null)
            _presetsButton.Pressed += OnPresetsPressed;
            
        if (_savedButton != null)
            _savedButton.Pressed += OnSavedPressed;
    }

    private void OnPresetsPressed()
    {
        _galery.Init(CrosshairSetting.OpenPresets());
        _menu.Enter(_galery);
    }

    private void OnSavedPressed()
    {
        _galery.Init(CrosshairSetting.OpenSaved());
        _menu.Enter(_galery);
    }
}