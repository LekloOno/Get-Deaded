using Godot;

public partial class UI_CrosshairPresets : Node
{
    [Export] private UI_EscapeMenu      _menu   = null!;
    [Export] private UI_CrosshairGalery _galery = null!;
    [Export] private Button _presetsButton = null!;
    [Export] private Button _savedButton = null!;
    [Export] private Button _saveButton = null!;

    public override void _Ready()
    {
        _presetsButton.Pressed  += OnPresetsPressed;
        _savedButton.Pressed    += OnCustomPressed;
        _saveButton.Pressed     += OnSavePressed;
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

    private void OnSavePressed()
    {
        _galery.Init(CrosshairSetting.OpenSaved(), UI_CrosshairGalery.Mode.Save);
        _menu.Enter(_galery);
    }
}