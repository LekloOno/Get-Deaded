using System;
using Godot;

[GlobalClass]
public partial class UI_ScoreBoardManager : TabContainer
{
	[Export] private UI_ScoreBoard _easyBoard 	= null!;
	[Export] private UI_ScoreBoard _normalBoard = null!;
	[Export] private UI_ScoreBoard _hardBoard 	= null!;

	public override void _Ready()
	{
		TabChanged += OnTabChanged;
	}

	public void GdInit()
	{
		Init();
	}

	public void Init(Guid? guid = null, int rank = 1)
	{
		_easyBoard.Clean();
		_normalBoard.Clean();
		_hardBoard.Clean();

		E_EnemyDifficulty difficulty = E_DifficultyServer.Difficulty;
		UI_ScoreBoard initBoard = GetBoard(difficulty);

		int tab = (int) difficulty;
		
		if (CurrentTab != tab)
		{
			TabChanged -= OnTabChanged;
			CurrentTab = tab;
			TabChanged += OnTabChanged;
		}
		
		initBoard.InitializeAsync(difficulty, guid, rank);
	}

	private UI_ScoreBoard GetBoard(E_EnemyDifficulty difficulty)
	{
		return difficulty switch
		{
			E_EnemyDifficulty.EASY => _easyBoard,
			E_EnemyDifficulty.NORMAL => _normalBoard,
			E_EnemyDifficulty.HARD => _hardBoard,
			_ => _hardBoard,
		};
	}

	private void OnTabChanged(long tab)
	{
		E_EnemyDifficulty difficulty = (E_EnemyDifficulty)tab;

		GetBoard(difficulty).InitializeAsync(difficulty);
	}
}
