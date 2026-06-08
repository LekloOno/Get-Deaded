using Godot;

[GlobalClass]
public partial class UI_CrosshairCross : Control
{
    private CrosshairCrossData _data = null!;

    [Export] private Range _length = null!;
    [Export] private Range _thickness = null!;
    [Export] private Range _gap = null!;
    [Export] private UI_CrosshairCrossArms _arms = null!;

    public override void _Ready()
    {
        _length.MinValue = _thickness.MinValue = _gap.MinValue = 0f;

        _length.MaxValue    = 30f;
        _thickness.MaxValue = 10f;
        _gap.MaxValue       = 10f;

        _length.ValueChanged    += OnLengthChanged;
        _thickness.ValueChanged += OnThicknessChanged;
        _gap.ValueChanged       += OnGapChanged;
    }

    public void SetData(CrosshairCrossData data)
    {
        _data = data;

        _length.Value    = data.Length;
        _thickness.Value = data.Thickness;
        _gap.Value       = data.Gap;

        _arms.SetData(data);
    }

    private void OnLengthChanged(double value) =>
        _data.Length    = (float) value;

    private void OnThicknessChanged(double value) =>
        _data.Thickness = (float) value;

    private void OnGapChanged(double value) =>
        _data.Gap       = (float) value;
}