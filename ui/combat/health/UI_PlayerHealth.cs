using Godot;

[GlobalClass]
public partial class UI_PlayerHealth : VBoxContainer
{
    [Export] private GC_HealthManager _healthManager;
    [Export] private UI_HealthBar _higherBar;
    [Export] private UI_HealthBar _lowerBar;

    private GC_Health _lowerLayer;

    public override void _Ready()
    {
        if (_healthManager.TopHealthLayer.IsLowerLayer())
            _higherBar.Visible = false;

        float higherMax = _healthManager.TopHealthLayer.HigherMax();
        //float higherCurrent = _healthManager.TopHealthLayer.HigherCurrent();
        float lowerMax = _healthManager.TopHealthLayer.LowerMax();
        //float lowerCurrent = _healthManager.TopHealthLayer.LowerCurrent();

        _higherBar.InitBar(higherMax, higherMax);
        _lowerBar.InitBar(lowerMax, lowerMax);
        
        _healthManager.TopHealthLayer.OnDamage += Damage;
        _healthManager.TopHealthLayer.OnHeal += Heal;
    }

    public void Damage(GC_Health senderLayer, DamageEventArgs damageArgs)
    {
        if (senderLayer.IsLowerLayer())
            _lowerBar.Damage(_healthManager.TopHealthLayer.LowerCurrent());
        else
            _higherBar.Damage(_healthManager.TopHealthLayer.HigherCurrent());
    }

    public void Heal(GC_Health senderLayer, DamageEventArgs damageArgs)
    {
        if (senderLayer.IsLowerLayer())
            _lowerBar.Heal(_healthManager.TopHealthLayer.LowerCurrent());
        else
            _higherBar.Heal(_healthManager.TopHealthLayer.HigherCurrent());
    }
}