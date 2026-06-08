using Godot;

[GlobalClass]
public partial class UI_CrosshairOutlineData : UI_CrosshairFillData
{
    private OutlineData _outlineData = null!;

    [Export] private Control _widthSetting = null!;
    [Export] private Range   _widthRange = null!;

    public override void _Ready()
    {
        base._Ready();
        _widthRange.ValueChanged += OnWidthChanged;
    }

    public void SetOutlineData(OutlineData outline)
    {
        _outlineData = outline;
        
        _widthRange.Value = outline.Width;

        SetData(outline);
    }

    private void OnWidthChanged(double value) =>
        _outlineData.Width = (float) value;

    protected override void UpdateVisibility()
    {
        base.UpdateVisibility();
        _widthSetting.Visible = _outlineData.Visible;
    }
}