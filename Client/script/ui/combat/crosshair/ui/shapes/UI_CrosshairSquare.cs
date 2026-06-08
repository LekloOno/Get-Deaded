using Godot;

[GlobalClass]
public partial class UI_CrosshairSquare : Control
{
    private CrosshairSquareData _data = null!;

    [Export] private Range _size = null!;

    [Export] private CheckBox _filled = null!;
    [Export] private Range _thickness = null!;
    [Export] private Control _thicknessSetting = null!;

    public override void _Ready()
    {
        _size.MinValue = _thickness.MinValue = 0f;

        _size.MaxValue      = 30f;
        _thickness.MaxValue = 10f;

        _size.ValueChanged      += OnSizeChanged;

        _filled.Toggled         += OnFilledToggled;
        _thickness.ValueChanged += OnThicknessChanged;
    }

    public void SetData(CrosshairSquareData data)
    {
        _data = data;

        _size.Value = data.Size;

        _filled.ButtonPressed = data.Filled;
        _thickness.Value      = data.Thickness;

        UpdateVisibility();
    }

    private void UpdateVisibility() =>
        _thicknessSetting.Visible = !_data.Filled;

    private void OnSizeChanged(double value) =>
        _data.Size = (float) value;

    private void OnFilledToggled(bool toggledOn)
    {
        _data.Filled = toggledOn;
        UpdateVisibility();
    }

    private void OnThicknessChanged(double value) =>
        _data.Thickness = (float) value;
}