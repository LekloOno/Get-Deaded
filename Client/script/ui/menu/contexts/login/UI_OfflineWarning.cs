using System;
using Client.Api.Auth;
using Godot;

public partial class UI_OfflineWarning : Control
{
	[Export] private UI_EscapeMenu? _escapeMenu;
	[Export] private Control? _difficultyMenu;
	[Export] private Button? _offlineButton;
	[Export] private Button? _loginButton;
	[Export] private Button? _registerButton;

	[Signal]
	public delegate void LoginEventHandler();

	[Signal]
	public delegate void RegisterEventHandler();

	public override void _Ready()
	{
		_offlineButton.Pressed += OnOfflinePressed;
		_loginButton.Pressed += OnLoginPressed;
		_registerButton.Pressed += OnRegisterPressed;
	}

	private void OnRegisterPressed()
	{
		_escapeMenu.ExitCurrent();
		EmitSignal(SignalName.Register);
	}

	private void OnLoginPressed()
	{
		_escapeMenu.ExitCurrent();
		EmitSignal(SignalName.Login);
	}

	private void OnOfflinePressed()
	{
		_escapeMenu.ExitCurrent();
		Session.Offline = true;
		_escapeMenu.Enter(_difficultyMenu);
	}

	public void EmitPlay() =>
		EmitSignal(SignalName.Login);
}
