using Godot;

public partial class UI_DoubleJumpMode : Node
{
    [Export] private Button _dediButton = null!;
    [Export] private Button _jumpButton = null!;
    [Export] private Button _dashButton = null!;

    public override void _Ready()
    {
        _dediButton.Pressed += OnButtonPressed;
        _jumpButton.Pressed += OnButtonPressed;
        _dashButton.Pressed += OnButtonPressed;

        SyncButtons(DoubleJumpModeSetting.Mode);
        DoubleJumpModeSetting.ModeChanged += SyncButtons;
    }

    private void OnButtonPressed()
    {
        if (!DoubleJumpModeSetting.Instance.TryUpdateValue(this, (int) SelectedMode(), out _))
            SyncButtons(DoubleJumpModeSetting.Mode);
    }

    private void SyncButtons(DoubleJumpMode mode)
    {
        _dediButton.SetPressedNoSignal(mode.HasFlag(DoubleJumpMode.Defined));
        _dashButton.SetPressedNoSignal(mode.HasFlag(DoubleJumpMode.DashJump));
        _jumpButton.SetPressedNoSignal(mode.HasFlag(DoubleJumpMode.HeightJump));
    }

    private DoubleJumpMode SelectedMode()
    {
        DoubleJumpMode mode = 0;
        if (_dediButton.ButtonPressed)
            mode |= DoubleJumpMode.Defined;
        if (_jumpButton.ButtonPressed)
            mode |= DoubleJumpMode.HeightJump;
        if (_dashButton.ButtonPressed)
            mode |= DoubleJumpMode.DashJump;

        return mode;
    }
}