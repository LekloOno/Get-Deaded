using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Api;
using Client.Api.Godot;
using Godot;
using Shared.Scores;

[GlobalClass]
public partial class UI_ScoreBoard : Control
{
    // Should be the only child of _easyContainer.
    [Export] private UI_ScoreBoardEntry _entryTemplate;
    [Export] private Control _container;

    private List<UI_ScoreBoardEntry> _entries = [];

    public bool Initialized {get; private set;} = false;

    public override void _Ready()
    {
        _entryTemplate.Visible = false;
        _entryTemplate.SetProcess(false);
    }

    public async Task InitializeAsync(E_EnemyDifficulty difficulty, Guid? guid = null, int Rank = 1)
    {
        if (Initialized)
            return;

        Initialized = true;
        
        ApiResult<List<LeaderboardRowDto>> result = await ApiGodotGlue.Instance.ScoreApi.GetLeaderboardAsync(
            "dust_pit",
            (int)difficulty,
            Rank
        );
        
        if (result.Success && result.Data != null)
            CreateEntries(result.Data, guid);
    }

    private void CreateEntries(List<LeaderboardRowDto> rows, Guid? guid)
    {
        foreach (LeaderboardRowDto row in rows)
        {
            UI_ScoreBoardEntry entry = (UI_ScoreBoardEntry) _entryTemplate.Duplicate();
            
            if (row.ScoreId == guid)
                entry.ThemeTypeVariation = "NewEntryPannel";
            else
                entry.ThemeTypeVariation = "EntryPannel";

            entry.Visible = true;
            entry.SetProcess(true);
            entry.Initialize(row);
            _container.AddChild(entry);
            _entries.Add(entry);
        }
    }

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