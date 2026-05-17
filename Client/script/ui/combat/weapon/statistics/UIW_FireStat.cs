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

    private bool _staticBind;

    private STAT_Fire _fire;

    private bool _boundToData = false;

    public void Initialize(STAT_Fire fire, bool staticBind = false)
    {
        _fire = fire;
        _staticBind = staticBind;
        
        _icon.Texture = _fire.Icon;
        _icon.Modulate = _fire.IconColor;

        if (staticBind)
            StaticInitialize();

        Bind();
    }

    private void StaticInitialize()
    {
        UpdateKills(_fire.Kills);
        UpdateDamage(_fire.Damage);
        UpdateAccuracy(0);
    }
    
    private void UpdateAccuracy(int arg)
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

    private void UpdateCritical(int criticals)
    {
        float hits = (float) _fire.Hits;
        _critical.Text = Math.Round(criticals * 100f / hits) + " %";
    }

    private void UpdateDamage(float damage) =>
        _damages.Text = Math.Round(damage) + "";

    private void UpdateKills(int kills) =>
        _kills.Text = kills + "";

    private void Bind()
    {
		if (_staticBind)
			return;

        if (_fire == null)
            return;

        if (_boundToData)
            return;

        _fire.Shots.Subscribe(UpdateAccuracy);
        _fire.Hits.Subscribe(UpdateAccuracy);
        _fire.Damage.Subscribe(UpdateDamage);
        _fire.Kills.Subscribe(UpdateKills);
        _boundToData = true;
    }

    private void Unbind()
    {
		if (_staticBind)
			return;


        if (_fire == null)
            return;

        if (!_boundToData)
            return;

        _fire.Shots.Unsubscribe(UpdateAccuracy);
        _fire.Hits.Unsubscribe(UpdateAccuracy);
        _fire.Damage.Unsubscribe(UpdateDamage);
        _fire.Kills.Unsubscribe(UpdateKills);
        _boundToData = false;
    }
}