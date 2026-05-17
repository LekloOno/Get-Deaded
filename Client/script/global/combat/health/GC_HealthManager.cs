using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class GC_HealthManager : Node3D
{
	[Export] public GB_ExternalBodyManagerWrapper _body;
	
	private GC_Health _topHealthLayer;
	[Export] public GC_Health TopHealthLayer
	{
		get => _topHealthLayer;
		set
		{
			if (_topHealthLayer == value)
				return;

			if (_topHealthLayer != null)
			{
				_topHealthLayer.OnDie -= Die;
				_topHealthLayer.OnDamage -= PropagDamage;
				_topHealthLayer.OnHeal -= PropagHeal;
				_topHealthLayer.OnBreak -= PropagBreak;
				_topHealthLayer.OnFull -= PropagFull;
			}
			
			_topHealthLayer = value;
			if (_topHealthLayer == null)
				return;

			_topHealthLayer.OnDie += Die;
			_topHealthLayer.OnDamage += PropagDamage;
			_topHealthLayer.OnHeal += PropagHeal;
			_topHealthLayer.OnBreak += PropagBreak;
			_topHealthLayer.OnFull += PropagFull;
			Init();
		}
	}

	[Export] private Array<GC_HurtBox> _hurtBoxes = [];
	[Export] public CONF_HurtBoxFaction HurtMask {get; private set;} = CONF_HurtBoxFaction.Enemy;
	public HealthInitEventArgs InitState {get; private set;} = null;

	public EventHandler<HealthInitEventArgs> OnLayerInit;

	public virtual bool Damage(
		GC_IHitDealer hitDealer,
		float expectedDamage,
		out float takenDamage,
		out float overflow,
		out GC_Health deepest
	) => TopHealthLayer.TakeDamage(expectedDamage, out takenDamage, out overflow, out deepest);

	public float Heal(float heal) => TopHealthLayer.Heal(heal, null);
	public override void _Ready()
	{
		EnableHurt(HurtMask.LayerMask());
	}

	public void Die(GC_Health sender)
	{
		DisableHurt();
		TopHealthLayer.Disable();
		OnDie?.Invoke(sender);
	}

	public void Init(bool reInit = false)
	{
		EnableHurt(HurtMask.LayerMask());
		TopHealthLayer.Initialize(out float totalInit, out float lowerInit, out float totalMax, out float lowerMax);
		InitState = new(totalInit, lowerInit, totalMax, lowerMax, reInit);
		OnLayerInit?.Invoke(this, InitState);
	}
	public GC_Health GetLowerLayer() => TopHealthLayer.GetLowerLayer();
	public GC_Health GetExposedLayer() => TopHealthLayer.GetExposedLayer();

	public void EnableHurt(uint collisionLayer)
	{
		foreach (GC_HurtBox hurtBox in _hurtBoxes)
			hurtBox.CollisionLayer = collisionLayer;
	}

	public void DisableHurt()
	{
		foreach (GC_HurtBox hurtBox in _hurtBoxes)
			hurtBox.CollisionLayer = 0;
	}

	public void HandleKnockBack(Vector3 force)
	{
		_body.HandleKnockBack(force);
	}




	public HealthEventHandler OnDie;
	public HealthEventHandler<DamageEventArgs> OnDamage;
	private void PropagDamage(GC_Health sender, DamageEventArgs damage) =>
		OnDamage?.Invoke(sender, damage);

	public HealthEventHandler<DamageEventArgs> OnHeal;
	private void PropagHeal(GC_Health sender, DamageEventArgs heal) =>
		OnHeal?.Invoke(sender, heal);

	public HealthEventHandler<GC_Health> OnBreak;
	private void PropagBreak(GC_Health sender, GC_Health childLayer) =>
		OnBreak?.Invoke(sender, childLayer);

	public HealthEventHandler<GC_Health> OnFull;
	private void PropagFull(GC_Health sender, GC_Health parentLayer) =>
		OnFull?.Invoke(sender, parentLayer);
}
