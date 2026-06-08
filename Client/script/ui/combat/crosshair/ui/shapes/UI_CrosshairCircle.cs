using Godot;

[GlobalClass]
public partial class UI_CrosshairCircle : Control
{
    private CrosshairCircleData _data = null!;

    [Export] private Range    _radius = null!;
    [Export] private Range    _thickness = null!;
    [Export] private Range    _points = null!;

    [Export] private CheckBox _customArc = null!;
    [Export] private Range    _startAngle = null!;
    [Export] private Range    _endAngle = null!;
    [Export] private Control  _startAngleSetting = null!;
    [Export] private Control  _endAngleSetting = null!;

    public override void _Ready()
    {
        _radius.MinValue = _thickness.MinValue
        = _startAngle.MinValue = _endAngle.MinValue = 0f;

        _points.MinValue = 4;

        _radius.MaxValue     = 30f;
        _thickness.MaxValue  = 10f;
        _startAngle.MaxValue = _endAngle.MaxValue = 360f;
        _points.MaxValue     = 64;

        _thickness.ValueChanged  += OnThicknessChanged;
        _radius.ValueChanged     += OnRadiusChanged;
        _points.ValueChanged     += OnPointsChanged;

        _customArc.Toggled       += OnCustomArcToggled;
        _startAngle.ValueChanged += OnStartAngleChanged;
        _endAngle.ValueChanged   += OnEndAngleChanged;
    }

    public void SetData(CrosshairCircleData data)
    {
        _data = data;

        _thickness.Value = data.Thickness;
        _radius.Value    = data.Radius;
        _points.Value    = data.Points;

        _customArc.ButtonPressed = data.CustomArc;
        _startAngle.Value = data.StartAngle;
        _endAngle.Value   = data.EndAngle;

        UpdateVisibility();
    }

    private void UpdateVisibility() =>
        _startAngleSetting.Visible = _endAngleSetting.Visible = _data.CustomArc;

    private void OnRadiusChanged(double value) =>
        _data.Radius = (float) value;

    private void OnThicknessChanged(double value) =>
        _data.Thickness = (float) value;

    private void OnPointsChanged(double value) =>
        _data.Points = (int) value;

    private void OnCustomArcToggled(bool toggledOn)
    {
        _data.CustomArc = toggledOn;
        UpdateVisibility();
    }

    private void OnStartAngleChanged(double value) =>
        _data.StartAngle = (float) value;

    private void OnEndAngleChanged(double value) =>
        _data.EndAngle = (float) value;
}