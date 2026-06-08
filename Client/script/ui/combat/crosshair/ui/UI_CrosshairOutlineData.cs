using Godot;

[GlobalClass]
public partial class UI_CrosshairOutlineData : UI_CrosshairFillData
{
    private OutlineData _outlineData = null!;

    [Export] private Range _widthRange = null!;

    public override void _Ready()
    {
        base._Ready();
        _widthRange.ValueChanged += OnWidthChanged;
    }

    public void SetOutlineData(OutlineData outline)
    {
        SetData(outline);

        _outlineData = outline;
        
        _widthRange.Value = outline.Width;

        UpdateWidthVisibility();
    }

    private void OnWidthChanged(double value) =>
        _outlineData.Width = (float) value;

    private void UpdateWidthVisibility() =>
        _widthRange.Visible = _outlineData.Visible;
}