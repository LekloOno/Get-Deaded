using System.Threading.Tasks;
using Client.Api.Score;
using Godot;

[GlobalClass]
public partial class UI_ScoreBoardManager : Control
{
    [Export] private TabContainer _difficultiesTab;
    [Export] private UI_ScoreBoard _easyBoard;
    [Export] private UI_ScoreBoard _normalBoard;
    [Export] private UI_ScoreBoard _hardBoard;

    public override void _Ready()
    {
        _difficultiesTab.TabChanged += OnTabChanged;
    }

    public async Task Init(ScoreResult result)
    {
        _easyBoard.Clean();
        _normalBoard.Clean();
        _hardBoard.Clean();

        UI_ScoreBoard initBoard = GetBoard(E_DifficultyServer.Difficulty);
        await initBoard.InitializeAsync(result.Rank ?? 1);
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

        GetBoard(difficulty).InitializeAsync();
    }
}