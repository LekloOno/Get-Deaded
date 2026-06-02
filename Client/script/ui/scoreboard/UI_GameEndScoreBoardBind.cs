using System;
using Client.Api.Auth;
using Godot;

public partial class UI_GameEndScoreBoardBind : Node
{
    [Export] private UI_EscapeMenu _escapeMenu;
    [Export] private Control _scoreBoardMenu;
    [Export] private UI_ScoreBoardManager _scoreBoardManager;
    [Export] private SC_GameManager _gameManager;
    [Export] private UIW_ArenaEnd _briefScore;

    private bool _entered = false;

    public override void _Ready()
    {
        _gameManager.ScoreSubmitted += OnScoreSubmitted;
        _gameManager.ResetGame += OnReset;
        _scoreBoardManager.VisibilityChanged += OnScoreBordVisibilityChange;
    }

    private void OnScoreBordVisibilityChange()
    {
        if (!_scoreBoardManager.IsVisibleInTree())
            _entered = false;
    }

    private void OnReset()
    {
        _escapeMenu.SetUnpausable();
        _escapeMenu.Open();
        if (Session.IsAuthenticated)
            OpenScoreBoard();
        else
            _briefScore.ShowBrief();
    }

    public void OpenScoreBoard()
    {
        _escapeMenu.Enter(_scoreBoardMenu);
        _entered = true;
    }

    private void OnScoreSubmitted(Guid guid, int rank)
    {
        if (_entered)
            _scoreBoardManager.Init(guid, rank);
    }
}