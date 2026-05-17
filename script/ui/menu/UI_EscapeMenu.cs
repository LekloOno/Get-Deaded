using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class UI_EscapeMenu : Control, UI_IMenuStackManager
{
	[Export] private bool _pauseGame = true;
	[Export] private Control _baseMenu;
	private bool _enabled = true;
	private Input.MouseModeEnum _prevMouseMode;
	public bool Enabled
	{
		get => _enabled;
		set
		{
			if (_enabled == value)
				return;

			_enabled = value;
			if (!_enabled)
				Close();
			
			SetProcessUnhandledKeyInput(_enabled);
		}
	}
	private Stack<Control> _menuStack = [];
	[Signal]
	public delegate void ClosedEventHandler();
	[Signal]
	public delegate void OpenedEventHandler();

	public void SetPausable() =>
		_pauseGame = true;

	public void SetUnpausable()
	{
		GetTree().Paused = false;
		_pauseGame = false;
	}

	public override void _Ready()
	{
		foreach (Node child in GetChildren())
			if (child is Control control)
				control.Visible = false;

		if (Visible)
			Open();
	}

	public override void _UnhandledKeyInput(InputEvent @event)
	{
		if (!@event.IsActionPressed("ui_cancel"))
			return;
		
		if (Visible)
			ExitCurrent();
		else
			Open();
	}

	public void Open()
	{
		_prevMouseMode = Input.MouseMode;
		Input.MouseMode = Input.MouseModeEnum.Visible;
		Show();
		Enter(_baseMenu);

		if (_pauseGame)
			GetTree().Paused = true;

		EmitSignal(SignalName.Opened);
	}

	public void Close()
	{
		Input.MouseMode = _prevMouseMode;
		if (_pauseGame)
			GetTree().Paused = false;

		if (_menuStack.TryPop(out Control menu))
			menu.Hide();
			
		_menuStack = [];
		Hide();
		EmitSignal(SignalName.Closed);
	}

	public void Enter(Control menu)
	{
		if (_menuStack.TryPeek(out Control current))
			current.Hide();

		_menuStack.Push(menu);
		menu.Show();
	}

	/// <summary>
	/// Exit the current menu, and returns true if it is still in a stacked menu.
	/// </summary>
	public void ExitCurrent()
	{
		Control current = _menuStack.Pop();
		current.Hide();

		if (_menuStack.TryPeek(out Control next))
			next.Show();
		else
			Close();
	}

	public void Enable() => Enabled = true;
	public void Disable() => Enabled = false;
}
