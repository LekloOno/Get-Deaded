using System;
using Godot;

namespace Pew;

[GlobalClass]
public partial class UI_EntityHealth : VBoxContainer
{
    [Export] private GC_HealthManager _healthManager;
    [Export] private UI_HealthBar _higherBar;
    [Export] private UI_HealthBar _lowerBar;

    private GC_Health _lowerLayer;

    public override void _Ready()
    {
        _healthManager.OnLayerInit += (o, initState) => InitState(initState);
        if (_healthManager.InitState != null)
            InitState(_healthManager.InitState);     
    }

    public virtual void InitState(HealthInitEventArgs initState)
    {
        float lowerMax = initState.LowerMaxHealth;
        float lowerInit = initState.LowerInitHealth;

        float higherMax = initState.TotalMaxHealth - lowerMax;
        float higherInit = initState.TotalInitHealth - lowerInit;

        CONFD_BarColors higherInitColor = CONF_HealthColors.GetBarColors(_healthManager.GetExposedLayer());
        CONFD_BarColors lowerInitColor = CONF_HealthColors.GetBarColors(_healthManager.GetLowerLayer());

        _higherBar.InitBar(higherMax, higherInit, higherInitColor);
        _lowerBar.InitBar(lowerMax, lowerInit, lowerInitColor);

        if (_healthManager.TopHealthLayer.IsLowerLayer())
            _higherBar.Visible = false;

        if (!initState.ReInit)
        {
            _healthManager.TopHealthLayer.OnDamage += Damage;
            _healthManager.TopHealthLayer.OnHeal += Heal;
            _healthManager.TopHealthLayer.OnBreak += Break;
            _healthManager.TopHealthLayer.OnFull += Full;
        }
    }

    private void Full(GC_Health senderLayer, GC_Health nextLayer)
    {
        if (nextLayer == null)
            return;
        
        if (senderLayer.IsLowerLayer())
            return;

        CONFD_BarColors barColors = CONF_HealthColors.GetBarColors(nextLayer);
        _higherBar.Break(barColors);
    }

    private void Break(GC_Health senderLayer, GC_Health nextLayer)
    {
        if (nextLayer == null || nextLayer.IsLowerLayer())
            return;

        CONFD_BarColors barColors = CONF_HealthColors.GetBarColors(_healthManager.GetExposedLayer());
        _higherBar.Break(barColors);
    }

    public void Damage(GC_Health senderLayer, DamageEventArgs damageArgs)
    {
        if (senderLayer.IsLowerLayer())
            _lowerBar.Damage(damageArgs.CurrentHealth);
        else
            _higherBar.Damage(damageArgs.TotalCurrentHealth - damageArgs.LowerCurrentHealth);
    }

    public void Heal(GC_Health senderLayer, DamageEventArgs damageArgs)
    {
        if (senderLayer.IsLowerLayer())
            _lowerBar.Heal(damageArgs.CurrentHealth);
        else
            _higherBar.Heal(damageArgs.TotalCurrentHealth - damageArgs.LowerCurrentHealth);
    }
}