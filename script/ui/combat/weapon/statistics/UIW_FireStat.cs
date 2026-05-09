using System;
using Godot;

[GlobalClass]
public partial class UIW_FireStat : UIW_Stats
{
    [Export] private TextureRect _icon;
    [Export] private Label _kills;
    [Export] private Label _damages;
    [Export] private Label _accuracy;
    [Export] private Label _critical;

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
        _critical.Text = Math.Round((float) _fire.LocalHits[(int)GC_BodyPart.Head] * 100f / _fire.Hits) + " %";
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
        _fire.Damage.Subscribe(UpdateDamage);
        _fire.Kills.Subscribe(UpdateKills);
        _boundToData = true;
    }

    private void UpdateCritical(int criticals)
    {
        float hits = (float) _fire.Hits;
        _critical.Text = Math.Round(criticals * 100f / hits) + " %";
    }

    private void UpdateDamage(float damage) =>
        _damages.Text = Math.Round(damage) + "";

    private void UpdateKills(int kills) =>
        _kills.Text = kills + "";

    private void Unbind()
    {
        if (_fire == null)
            return;

        if (!_boundToData)
            return;

        _fire.Shots.Unsubscribe(UpdatePrecision);
        _fire.Hits.Unsubscribe(UpdatePrecision);
        _fire.Damage.Unsubscribe(UpdateDamage);
        _fire.Kills.Unsubscribe(UpdateKills);
        _boundToData = false;
    }
}