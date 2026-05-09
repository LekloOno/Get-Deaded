using System;
using Godot;

[GlobalClass]
public partial class UIW_FireStat : UIW_Stats
{
    [Export] private TextureRect _icon;
    [Export] private Label _accuracy;

    private STAT_Fire _fire;

    private bool _boundToData = false;

    public void Initialize(STAT_Fire fire)
    {
        _fire = fire;
        
        _icon.Texture = _fire.Icon;
        _icon.Modulate = _fire.IconColor;

        Bind();
    }
    
    private void UpdatePrecision(int arg)
    {
        _accuracy.Text = Math.Round((float) _fire.Hits * 100 / (float) _fire.Shots) + " %";
    }

    public override void _EnterTree()
    {
        Bind();
    }

    public override void _ExitTree()
    {
        Unbind();
    }

    private void Bind()
    {
        if (_fire == null)
            return;

        if (_boundToData)
            return;

        _fire.Shots.Subscribe(UpdatePrecision);
        _fire.Hits.Subscribe(UpdatePrecision);
        _boundToData = true;
    }

    private void Unbind()
    {
        if (_fire == null)
            return;

        if (!_boundToData)
            return;

        _fire.Shots.Unsubscribe(UpdatePrecision);
        _fire.Hits.Unsubscribe(UpdatePrecision);
        _boundToData = false;
    }
}