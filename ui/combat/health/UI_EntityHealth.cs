using Godot;

[GlobalClass]
public partial class UI_EntityHealth : VBoxContainer
{
    [Export] private GC_HealthManager _healthManager;
    [Export] private UI_HealthBar _higherBar;
    [Export] private UI_HealthBar _lowerBar;

    private GC_Health _lowerLayer;

    public override void _Ready()
    {
        if (_healthManager.InitState == null)
            _healthManager.OnLayerInit += (o, initState) => InitState(initState);
        else
            InitState(_healthManager.InitState);

/*
        float higherMax = _healthManager.TopHealthLayer.HigherMax();
        //float higherCurrent = _healthManager.TopHealthLayer.HigherCurrent();
        float lowerMax = _healthManager.TopHealthLayer.LowerMax();
        //float lowerCurrent = _healthManager.TopHealthLayer.LowerCurrent();
*/            
    }

    public void InitState(HealthInitEventArgs initState)
    {
        float lowerMax = initState.LowerMaxHealth;
        float lowerInit = initState.LowerInitHealth;

        float higherMax = initState.TotalMaxHealth - lowerMax;
        float higherInit = initState.TotalInitHealth - lowerInit;

        _higherBar.InitBar(higherMax, higherInit);
        _lowerBar.InitBar(lowerMax, lowerInit);

        if (_healthManager.TopHealthLayer.IsLowerLayer())
            _higherBar.Visible = false;

        _healthManager.TopHealthLayer.OnDamage += Damage;
        _healthManager.TopHealthLayer.OnHeal += Heal;
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