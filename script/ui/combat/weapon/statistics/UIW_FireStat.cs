using System;
using Godot;

[GlobalClass]
public partial class UIW_FireStat : UIW_Stats
{
    [Export] private TextureRect _icon;
    [Export] private Label _accuracy;

    private STAT_Fire _fire;

    public void Initialize(STAT_Fire fire)
    {
        _fire = fire;
        
        _icon.Texture = _fire.Icon;
        _icon.Modulate = _fire.IconColor;

        _fire.Shots.Subscribe(UpdatePrecision);
        _fire.Hits.Subscribe(UpdatePrecision);
    }
    
    private void UpdatePrecision(int arg)
    {
        _accuracy.Text = Math.Round((float) _fire.Hits * 100 / (float) _fire.Shots) + " %";
    }

    public override void _ExitTree()
    {
        if (_fire == null)
            return;
            
        _fire.Shots.Unsubscribe(UpdatePrecision);
        _fire.Hits.Unsubscribe(UpdatePrecision);
    }
}