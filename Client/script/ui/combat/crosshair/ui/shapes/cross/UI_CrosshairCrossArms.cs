using Godot;

[GlobalClass]
public partial class UI_CrosshairCrossArms : Control
{
    private CrosshairCrossData _data = null!;

    [Export] private Button _top    = null!;
    [Export] private Button _bottom = null!;
    [Export] private Button _right  = null!;
    [Export] private Button _left   = null!;

    public override void _Ready()
    {
        _top.Toggled    += OnArmsChanged;
        _bottom.Toggled += OnArmsChanged;
        _right.Toggled  += OnArmsChanged;
        _left.Toggled   += OnArmsChanged;
    }

    public void SetData(CrosshairCrossData data)
    {
        _data = data;
        
        _top.ButtonPressed    = data.Arms.HasFlag(CrosshairCrossData.ArmMask.Top);
        _bottom.ButtonPressed = data.Arms.HasFlag(CrosshairCrossData.ArmMask.Bottom);
        _right.ButtonPressed  = data.Arms.HasFlag(CrosshairCrossData.ArmMask.Right);
        _left.ButtonPressed   = data.Arms.HasFlag(CrosshairCrossData.ArmMask.Left);
    }

    private void OnArmsChanged(bool toggledOn)
    {
        CrosshairCrossData.ArmMask arms = 0;

        if (_top.ButtonPressed)
            arms |= CrosshairCrossData.ArmMask.Top;

        if (_bottom.ButtonPressed)
            arms |= CrosshairCrossData.ArmMask.Bottom;

        if (_right.ButtonPressed)
            arms |= CrosshairCrossData.ArmMask.Right;

        if (_left.ButtonPressed)
            arms |= CrosshairCrossData.ArmMask.Left;

        _data.Arms = arms;
    }
}