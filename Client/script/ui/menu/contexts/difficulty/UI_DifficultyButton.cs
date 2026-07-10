using Godot;
using Shared.Scores;

public partial class UI_DifficultyButton : Button
{
	[Export] private Difficulty _difficulty;

	public override void _Ready()
	{
		Text = "DIFFICULTY_" + _difficulty.ToString() + "_BUTTON";
		Pressed += OnPressed;
	}

	private void OnPressed() =>
		E_DifficultyServer.Difficulty = _difficulty;
}
