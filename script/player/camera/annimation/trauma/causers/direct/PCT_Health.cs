using Godot;

[GlobalClass]
public partial class PCT_Health : PCT_DirectCauser
{
    [Export] private GC_HealthManager _healthManager;
    [Export] MATH_FloatCurveSampler _curveSampler;

    public override void _Ready() =>
        _healthManager.TopHealthLayer.OnDamage += DamageTrauma;

    private void DamageTrauma(GC_Health senderLayer, DamageEventArgs e) =>
        _shakeableCamera.AddTrauma(_curveSampler.Sample(e.Amount));
}