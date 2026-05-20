using Client.Api.Auth;
using Godot;

public partial class UI_OfflineWarningGuard : Node
{
	[Export] private UI_EscapeMenu? _escapeMenu;
	[Export] private Control? _difficultyMenu;
	[Export] private UI_OfflineWarning? _offlineMenu;
	[Export] private Button? _triggerButton;

	public override void _Ready()
	{
		_triggerButton.Pressed += OnPressed;
	}

	private void OnPressed()
	{
		if (Session.IsAuthenticated || Session.Offline)
		{
			_escapeMenu.Enter(_difficultyMenu);
			return;
		}

		_escapeMenu.Enter(_offlineMenu);
	}
}
