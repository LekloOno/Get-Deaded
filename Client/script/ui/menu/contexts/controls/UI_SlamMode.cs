using Godot;

public partial class UI_SlamMode : Node
{
	[Export] private Button _dediButton = null!;
	[Export] private Button _crouchButton = null!;
	[Export] private Button _dashButton = null!;
	[Export] private Control _dediInput = null!;

	public override void _Ready()
	{
		_dediButton.Pressed += OnButtonPressed;
		_crouchButton.Pressed += OnButtonPressed;
		_dashButton.Pressed += OnButtonPressed;

		Sync(this, SlamModeSetting.Mode);
		SlamModeSetting.ValueChanged += Sync;
	}

	private void OnButtonPressed()
	{
		if (!SlamModeSetting.Instance.TryUpdateValue(this, (int) SelectedMode(), out _))
			Sync(this, SlamModeSetting.Mode);
	}

	private void Sync(GodotObject? _, SlamMode mode)
	{
		_dediButton.SetPressedNoSignal(mode.HasFlag(SlamMode.Defined));
		_dashButton.SetPressedNoSignal(mode.HasFlag(SlamMode.DashCrouch));
		_crouchButton.SetPressedNoSignal(mode.HasFlag(SlamMode.QuickCrouch));

		_dediInput.Visible = mode.HasFlag(SlamMode.Defined);
	}

	private SlamMode SelectedMode()
	{
		SlamMode mode = 0;
		if (_dediButton.ButtonPressed)
			mode |= SlamMode.Defined;
		if (_crouchButton.ButtonPressed)
			mode |= SlamMode.QuickCrouch;
		if (_dashButton.ButtonPressed)
			mode |= SlamMode.DashCrouch;

		return mode;
	}
}
