using System;
using Godot;

public partial class UI_DifficultyButton : Button
{
    [Export] private E_EnemyDifficulty _difficulty;

    public override void _Ready()
    {
        Text = "DIFFICULTY_" + _difficulty.ToString() + "_BUTTON";
        Pressed += OnPressed;
    }

    private void OnPressed() =>
        E_DifficultyServer.Difficulty = _difficulty;
}