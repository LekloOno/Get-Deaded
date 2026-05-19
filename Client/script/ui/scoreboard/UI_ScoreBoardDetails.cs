using System.Collections.Generic;
using Godot;
using Shared.Scores;

[GlobalClass]
public partial class UI_ScoreBoardDetails : Control
{
    // Should be the only child of _container.
    [Export] private UI_ScoreBoardDetailsEntry _template;
    [Export] private Container _container;

    private List<UI_ScoreBoardDetailsEntry> _entries = [];

    public bool Initialized {get; private set;} = false;

    public override void _Ready()
    {
        _template.Visible = false;
        _template.SetProcess(false);
    }

    public void Initialize(ScoreDto scoreDetails)
    {
        if (Initialized)
            return;

        Initialized = true;

        CreateEntries(scoreDetails);
    }

    private void CreateEntries(ScoreDto scoreDetails)
    {
        foreach (WeaponStatDto weapon in scoreDetails.WeaponStats)
        {
            UI_ScoreBoardDetailsEntry detailsEntry = (UI_ScoreBoardDetailsEntry) _template.Duplicate();
            detailsEntry.Initialize(weapon);
            _entries.Add(detailsEntry);
            _container.AddChild(detailsEntry);
            detailsEntry.Visible = true;
            detailsEntry.SetProcess(true);
        }
    }

    public void Clean()
    {
        foreach (UI_ScoreBoardDetailsEntry entry in _entries)
            entry.QueueFree();

        _entries.Clear();

        Initialized = false;
    }
}