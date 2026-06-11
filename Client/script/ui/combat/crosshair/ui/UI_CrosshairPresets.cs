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
            _savedButton.Pressed += OnCustomPressed;
    }

    private void OnPresetsPressed()
    {
        _galery.Init(CrosshairSetting.OpenPresets(), UI_CrosshairGalery.Mode.BrowsePresets);
        _menu.Enter(_galery);
    }

    private void OnCustomPressed()
    {
        _galery.Init(CrosshairSetting.OpenSaved(), UI_CrosshairGalery.Mode.BrowseCustom);
        _menu.Enter(_galery);
    }
}