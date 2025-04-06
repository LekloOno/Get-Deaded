using Godot;

[GlobalClass]
public partial class PH_Trauma : Node
{
    [Export] private PH_Manager _healthManager;
    [Export] private PC_Shakeable _shakeableCamera;
    [Export] private float _minTrauma = 0f;
    [Export] private float _minDamage = 0f;
    [Export] private float _maxTrauma = 1f;
    [Export] private float _maxDamage = 150f;

    public override void _Ready()
    {
        _healthManager.TopHealthLayer.OnDamage += DamageTrauma;
    }

    private void DamageTrauma(GC_Health senderLayer, DamageEventArgs e)
    {
        float damageRatio = (e.Amount - _minDamage) / (_maxDamage - _minDamage);
        damageRatio = Mathf.Clamp(damageRatio, 0f, 1f);
        float trauma = Mathf.Lerp(_maxTrauma, _minTrauma, damageRatio);
        _shakeableCamera.AddTrauma(trauma);
    }
}