using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Api;
using Client.Api.Auth;
using Client.Api.Godot;
using Godot;
using Shared.Scores;

[GlobalClass]
public partial class UI_ScoreBoard : Control
{
    // Should be the only child of _easyContainer.
    [Export] private UI_ScoreBoardEntry _entryTemplate = null!;
    [Export] private Control _container = null!;
    [Export] private AudioStreamPlayer _entryHover = null!;
    [Export] private AudioStreamPlayer _entryClick = null!;

    [Export] private UI_Loading? _loading;
    [Export] private Label? _errorMessage;

    private List<UI_ScoreBoardEntry> _entries = [];

    public bool Initialized {get; private set;} = false;

    public override void _Ready()
    {
        _errorMessage?.Hide();
        _entryTemplate.Visible = false;
        _entryTemplate.SetProcess(false);
    }

    public async Task InitializeAsync(E_EnemyDifficulty difficulty, Guid? optScoreId = null, int Rank = 1)
    {
        if (Initialized)
            return;

        Initialized = true;
        _errorMessage?.Hide();
        _loading?.StartLoading();

        ApiResult<List<LeaderboardRowDto>> result;
        if (optScoreId is Guid scoreId)
            result = await ApiGodotGlue.Instance.ScoreApi.GetLeaderboardNewScoreAsync(
                "dust_pit",
                (int)difficulty,
                scoreId
            );
        else
            result = await ApiGodotGlue.Instance.ScoreApi.GetLeaderboardAsync(
                "dust_pit",
                (int)difficulty,
                Rank
            );
        
        if (result.Success && result.Data != null)
            CreateEntries(result.Data, optScoreId);

        else if (_errorMessage != null)
        {
            _errorMessage.Show();
            _errorMessage.Text = result.ErrorMessage;
        }
        
        _loading?.StopLoading();
    }

    private void CreateEntries(List<LeaderboardRowDto> rows, Guid? scoreId)
    {
        bool foundPB = false;

        foreach (LeaderboardRowDto row in rows)
        {
            UI_ScoreBoardEntry entry = (UI_ScoreBoardEntry) _entryTemplate.Duplicate();
            if (!foundPB && row.PlayerId == Session.PlayerId)
            {
                foundPB = true;
                if (row.ScoreId == scoreId)
                    entry.ThemeTypeVariation = "PanelNewPB";
                else
                    entry.ThemeTypeVariation = "PanelPB";
            }
            else if (row.ScoreId == scoreId)
                entry.ThemeTypeVariation = "PanelNewEntry";
            else
                entry.ThemeTypeVariation = "PanelEntry";

            entry.Visible = true;
            entry.SetProcess(true);
            entry.Initialize(row);

            entry.MouseEntered += EntryOnMouseEntered;
            entry.GuiInput += EntryOnGuiInput;

            _container.AddChild(entry);
            _entries.Add(entry);
        }
    }

    private void EntryOnGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mb &&
            mb.ButtonIndex == MouseButton.Left &&
            mb.Pressed)
        {
            _entryClick.Play(0);
        }
    }

    private void EntryOnMouseEntered() => _entryHover.Play(0);

    public void Clean()
    {
        foreach (UI_ScoreBoardEntry entry in _entries)
        {
            entry.Clean();
            entry.QueueFree();
        }

        _entries.Clear();
        
        Initialized = false;
    }
}