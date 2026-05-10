using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class UIW_PlayerStat : UIW_Stats
{
	[Export] private UIW_WeaponStat _weaponStatTemplate;
	[Export] private Label _damageLabel;
	[Export] private Label _killsLabel;
	[Export] private Label _deathsLabel;
	[Export] private Label _scoreLabel;

	private List<UIW_WeaponStat> _weaponsStats = [];
	private STAT_Combat _combatStats;
	private Observable<uint> _score;
	private bool _boundToData = false;
	private bool _staticBind = true;

	private void UpdateDamage(float value) {_damageLabel.Text = Mathf.Floor(value) + "";}
	private void UpdateKills(int value) {_killsLabel.Text = value + "";}
	private void UpdateDeaths(int value) {_deathsLabel.Text = value + "";}
	private void UpdateScore(uint value) {_scoreLabel.Text = value + "";}

	public void Initialize(STAT_Combat combat, Observable<uint> score, bool staticBind = false)
	{
		_combatStats = combat;
		_score = score;
		_staticBind = staticBind;

		if (staticBind)
			StaticInitialize();
		
		Bind();

		if (_weaponStatTemplate.GetParent() is Node parent)
			parent.RemoveChild(_weaponStatTemplate);

		foreach (UIW_WeaponStat w in _weaponsStats)
			w.QueueFree();
		
		_weaponsStats.Clear();

		foreach (STAT_Weapon weapon in combat.Weapons)
		{
			UIW_WeaponStat stat = (UIW_WeaponStat) _weaponStatTemplate.Duplicate();
			stat.Initialize(weapon, staticBind);
			_weaponsStats.Add(stat);
			AddChild(stat);
		}
	}

	private void StaticInitialize()
	{
		UpdateKills(_combatStats.Kills);
		UpdateDamage(_combatStats.Damage);
		UpdateDeaths(_combatStats.Deaths);
		UpdateScore(_score.Value);
	}


	public override void _ExitTree()
	{
		Unbind();
	}

	public override void _EnterTree()
	{
		Bind();
	}


	private void Bind()
	{
		if (_staticBind)
			return;

		if (_combatStats == null)
			return;

		if (_boundToData)
			return;

		_combatStats.Damage.Subscribe(UpdateDamage);
		_combatStats.Kills.Subscribe(UpdateKills);
		_combatStats.Deaths.Subscribe(UpdateDeaths);
		_score.Subscribe(UpdateScore);
		_boundToData = true;
	}

	private void Unbind()
	{
		if (_staticBind)
			return;

		if (_combatStats == null)
			return;

		if (!_boundToData)
			return;

		_combatStats.Damage.Unsubscribe(UpdateDamage);
		_combatStats.Kills.Unsubscribe(UpdateKills);
		_combatStats.Deaths.Unsubscribe(UpdateDeaths);
		_score.Unsubscribe(UpdateScore);
		_boundToData = false;
	}
}
