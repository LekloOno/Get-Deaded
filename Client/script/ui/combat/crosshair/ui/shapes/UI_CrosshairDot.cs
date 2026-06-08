using Godot;

[GlobalClass]
public partial class UI_CrosshairDot : Control
{
    private CrosshairDotData _data = null!;

    [Export] private Range _radius = null!;

    public override void _Ready()
    {
        _radius.MinValue = 0f;
        _radius.MaxValue = 30f;
        _radius.Step     = 0.1f;

        _radius.ValueChanged += OnRadiusChanged;
    }

    public void SetData(CrosshairDotData data)
    {
        _data = data;

        _radius.Value = data.Radius;
    }

    private void OnRadiusChanged(double value) =>
        _data.Radius = (float) value;
}