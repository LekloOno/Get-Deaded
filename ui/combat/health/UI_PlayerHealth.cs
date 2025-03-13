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

        //GD.Print(higherMax + " | " + higherCurrent);

        _higherBar.InitBar(higherMax, higherMax);
        _lowerBar.InitBar(lowerMax, lowerMax);
        
        _healthManager.TopHealthLayer.OnDamage += Damage;
        _healthManager.TopHealthLayer.OnHeal += Heal;
    }

    public void Damage(GC_Health senderLayer, float damage)
    {
        if (senderLayer.IsLowerLayer())
            _lowerBar.Damage(damage);
        else
            _higherBar.Damage(damage);
    }

    public void Heal(GC_Health senderLayer, float heal)
    {
        if (senderLayer.IsLowerLayer())
            _lowerBar.Heal(heal);
        else
            _higherBar.Heal(heal);
    }

    public override void _PhysicsProcess(double delta)
    {
        // To implement
    }
    public override void _Process(double delta)
    {
        // To implement
    }
}