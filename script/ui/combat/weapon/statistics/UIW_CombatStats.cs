using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class UIW_CombatStats : UIW_Stats
{
    [Export] private UIW_PlayerStat _uiPlayerStatTemplate;
    [Export] private Array<STAT_CombatTracker> _playersStats = [];
    [Export] private PI_Stats _statsInput;
    private readonly List<UIW_PlayerStat> _uiPlayerStats = [];

    public override void _Ready()
    {
        Hide();

        _statsInput.Start += (o, e) => Show();
        _statsInput.Stop += (o, e) => Hide();
        
        if (_playersStats.Count == 0)
            return;
        
        STAT_CombatTracker c = _playersStats.ElementAt(0);
        
        if (c.Initialized)
            Initialize();
        else
            c.GotInitialized += Initialize;
    }

    private void Initialize()
    {
        if (_uiPlayerStatTemplate.GetParent() is Node parent)
            parent.RemoveChild(_uiPlayerStatTemplate);

        foreach (UIW_PlayerStat w in _uiPlayerStats)
            w.QueueFree();
        
        _uiPlayerStats.Clear();

        foreach (STAT_CombatTracker combat in _playersStats)
        {
            UIW_PlayerStat stat = (UIW_PlayerStat) _uiPlayerStatTemplate.Duplicate();
            stat.Initialize(combat.Data);
            _uiPlayerStats.Add(stat);
            AddChild(stat);
        }
    }
}