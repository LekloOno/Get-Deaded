using Godot;

public partial class UIW_ArenaEnd : Node
{
	[Export] private UI_EscapeMenu _escapeMenu;
	[Export] private Control _combatStats;
	[Export] private PI_Stats _statsInput;
	[Export] private Button _returnButton;
	[Export] private Control _lastResultLabel;


	public override void _Ready()
	{
		_statsInput.Start += (o, e) => _combatStats.Show();
		_statsInput.Stop += (o, e) => _combatStats.Hide();
	}

	public void ShowScores()
	{
		_escapeMenu.Enter(_combatStats);
	}

	public void ShowBrief()
	{
		_lastResultLabel.Visible = true;
		_returnButton.Visible = true;
		ShowScores();
	}
}
