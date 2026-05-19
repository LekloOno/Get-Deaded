using System.Threading.Tasks;
using Client.Api.Score;
using Godot;

[GlobalClass]
public partial class UI_ScoreBoardManager : TabContainer
{
    [Export] private UI_ScoreBoard _easyBoard;
    [Export] private UI_ScoreBoard _normalBoard;
    [Export] private UI_ScoreBoard _hardBoard;

    public override void _Ready()
    {
        TabChanged += OnTabChanged;
    }

    public void Init()
    {
        Init(null);
    }

    public async Task Init(ScoreResult? result)
    {
        _easyBoard.Clean();
        _normalBoard.Clean();
        _hardBoard.Clean();

        GD.Print(E_DifficultyServer.Difficulty);

        E_EnemyDifficulty difficulty = E_DifficultyServer.Difficulty;
        UI_ScoreBoard initBoard = GetBoard(difficulty);
        await initBoard.InitializeAsync(difficulty, result?.Rank ?? 1);
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
        GD.Print("change to " + tab);
        E_EnemyDifficulty difficulty = (E_EnemyDifficulty)tab;

        GetBoard(difficulty).InitializeAsync(difficulty);
    }
}