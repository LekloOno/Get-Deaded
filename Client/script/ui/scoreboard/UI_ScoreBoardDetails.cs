using Godot;
using Shared.Scores;

[GlobalClass]
public partial class UI_ScoreBoardDetails : Control
{
    // Should be the only child of _container.
    [Export] private UI_ScoreBoardDetailsEntry _template;
    [Export] private Control _container;

    public bool Initialized {get; private set;} = false;

    public override void _Ready()
    {
        _container.RemoveChild(_container);
        Clean();
    }

    public void Initialize(ScoreDto scoreDetails)
    {
        Clean();
        CreateEntries(scoreDetails);
    }

    private void CreateEntries(ScoreDto scoreDetails)
    {
        foreach (WeaponStatDto weapon in scoreDetails.WeaponStats)
        {
            UI_ScoreBoardDetailsEntry detailsEntry = (UI_ScoreBoardDetailsEntry) _template.Duplicate();
            detailsEntry.Initialize(weapon);
            _container.AddChild(detailsEntry);
        }
    }

    public void Clean()
    {
        foreach (Node child in _container.GetChildren())
            child.QueueFree();
    }
}