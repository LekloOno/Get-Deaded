using Godot;

[GlobalClass]
public partial class UI_CrosshairFillData : Control
{
    private FillData _data = null!;

    [Export] private Control           _colorSetting = null!;
    [Export] private Control           _antiAliasSetting = null!;
    [Export] private CheckBox          _visible = null!;
    [Export] private ColorPickerButton _colorPicker = null!;
    [Export] private CheckBox          _antiAlias = null!;

    public override void _Ready()
    {
        _visible.Toggled          += OnVisibleToggled;
        _colorPicker.ColorChanged += OnColorChanged;
        _antiAlias.Toggled        += OnAntiAliasToggled;
    }

    public void SetData(FillData data)
    {
        _data = data;

        _colorPicker.Color = data.Color;
        _antiAlias.ButtonPressed = data.AntiAlias;
        _visible.ButtonPressed = data.Visible;

        UpdateVisibility();
    }

    private void OnVisibleToggled(bool toggledOn)
    {
        if (_data == null)
            return;

        _data.Visible = toggledOn;
        UpdateVisibility();
    }

    protected virtual void UpdateVisibility() =>
        _colorSetting.Visible = _antiAliasSetting.Visible = _data.Visible;

    private void OnAntiAliasToggled(bool toggledOn) =>
        _data.AntiAlias = toggledOn;

    private void OnColorChanged(Color color) =>
        _data.Color = color;
}