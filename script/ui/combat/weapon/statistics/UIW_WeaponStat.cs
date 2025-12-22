using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class UIW_WeaponStat : UIW_Stats
{
    [Export] private UIW_FireStat _fireStatTemplate;
    private readonly List<UIW_FireStat> _firesStat = [];

    public void Initialize(STAT_Weapon weapon)
    {
        if (_fireStatTemplate.GetParent() is Node parent)
            parent.RemoveChild(_fireStatTemplate);

        foreach (UIW_FireStat f in _firesStat)
            f.QueueFree();
        
        _firesStat.Clear();

        foreach (STAT_Fire fire in weapon.Fires)
        {
            UIW_FireStat stat = (UIW_FireStat) _fireStatTemplate.Duplicate();
            stat.Initialize(fire);
            _firesStat.Add(stat);
            AddChild(stat);
        }
    }
}