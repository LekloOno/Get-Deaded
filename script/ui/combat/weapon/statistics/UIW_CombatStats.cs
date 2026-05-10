using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class UIW_CombatStats : UIW_Stats
{
    [Export] private UIW_PlayerStat _uiPlayerStatTemplate;
    private Node _anchor;
    private List<UIW_PlayerStat> _uiPlayerStats = [];

    public override void _Ready()
    {
        if (_uiPlayerStatTemplate.GetParent() is Node parent)
        {
            parent.RemoveChild(_uiPlayerStatTemplate);
            _anchor = parent;
        } else
            _anchor = this;

        Clear();
    }

    public void Clear()
    {
        foreach (UIW_PlayerStat w in _uiPlayerStats)
            w.QueueFree();
        
        _uiPlayerStats.Clear();
    }

    public void AddStat(STAT_Combat data, Observable<uint> score)
    {
        UIW_PlayerStat stat = (UIW_PlayerStat) _uiPlayerStatTemplate.Duplicate();
        stat.Initialize(data, score);
        _uiPlayerStats.Add(stat);
        _anchor.AddChild(stat);
    }
}