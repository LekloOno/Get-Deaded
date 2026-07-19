using System;
using Client.Api.Auth;
using Godot;

public partial class UI_GameEndScoreBoardBind : Node
{
    [Export] private UI_EscapeMenu          _escapeMenu = null!;
    [Export] private Control                _scoreBoardMenu = null!;
    [Export] private UI_ScoreBoardManager   _scoreBoardManager = null!;
    [Export] private SC_GameMode         _gameManager = null!;
    [Export] private SC_ScoreManager        _scoreManager = null!;
    [Export] private UIW_ArenaEnd           _briefScore = null!;

    private bool _entered = false;

    public override void _Ready()
    {
        _scoreManager.ScoreSubmitted += OnScoreSubmitted;
        _gameManager.Reseted += OnReset;
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