using Godot;

[GlobalClass]
public partial class PH_Trauma : Node
{
    [Export] private PH_Manager _healthManager;
    [Export] private PC_Shakeable _shakeableCamera;
    [Export] MATH_FloatCurveSampler _curveSampler;

    public override void _Ready() =>
        _healthManager.TopHealthLayer.OnDamage += DamageTrauma;

    private void DamageTrauma(GC_Health senderLayer, DamageEventArgs e) =>
        _shakeableCamera.AddTrauma(_curveSampler.Sample(e.Amount));
}