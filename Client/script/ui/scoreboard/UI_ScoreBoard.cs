using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Client.Api.Godot;
using Client.Api.Score;
using Godot;
using Shared.Scores;

[GlobalClass]
public partial class UI_ScoreBoard : Control
{
    // Should be the only child of _easyContainer.
    [Export] private UI_ScoreBoardEntry _entryTemplate;
    [Export] private Control _container;

    public bool Initialized {get; private set;} = false;

    public override void _Ready()
    {
        _container.RemoveChild(_container);
        Clean();
    }

    public async Task InitializeAsync(int Rank = 1)
    {
        if (Initialized)
            return;

        Initialized = true;
        
        List<LeaderboardRowDto>? rows = await ApiGodotGlue.Instance.ScoreApi.GetLeaderboardAsync(
            "dust_pit",
            (int)E_DifficultyServer.Difficulty,
            Rank
        );
        
        if (rows != null)
            CreateEntries(rows);
    }

    private void CreateEntries(List<LeaderboardRowDto> rows)
    {
        foreach (LeaderboardRowDto row in rows)
        {
            UI_ScoreBoardEntry entry = (UI_ScoreBoardEntry) _entryTemplate.Duplicate();
            entry.Initialize(row);
            _container.AddChild(entry);
        }
    }

    public void Clean()
    {
        foreach (Node child in _container.GetChildren())
            child.QueueFree();
        
        Initialized = false;
    }
}