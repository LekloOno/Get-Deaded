using Godot;

public partial class VFX_LowHealth : ColorRect
{
	[Export] private GC_HealthManager _healthManager = null!;
	[Export] private float _healthThreshold = 70f;
	[Export] private float _maxIntensity = 0.7f;
	[Export] private Curve _interpolationCurve = null!;
	[Export] private ANIM_InOutTweenSetting _hitTweenSetting = null!;

	private Tween? _hitTween;

	private float _intensity = -1;

	public float Intensity
	{
		get => _intensity;
		set
		{
			if (value == _intensity)
				return;

			_intensity = value;
			SetInstanceShaderParameter("intensity", value);
			Visible = _intensity > 0f;
		}
	}

	public override void _Ready()
	{
		_healthManager.OnDamage += OnDamage;
		_healthManager.OnHeal += OnHeal;
		_healthManager.OnLayerInit += OnLayerInit;
		Intensity = 0f;
	}

	private void OnLayerInit(object? sender, HealthInitEventArgs e)
	{
		SetIntensity();
	}

	private void OnHeal(GC_Health senderLayer, DamageEventArgs e)
	{
		SetIntensity();
	}

	private void OnDamage(GC_Health senderLayer, DamageEventArgs e)
	{
		float health = _healthManager.TopHealthLayer.GetLowerLayer().CurrentHealth;
		float targetIntensity = IntensityFromHealth(health) * _maxIntensity;

		_hitTween?.Kill();
		
		if (targetIntensity == 0)
		{
			Intensity = 0;
			return;
		}

		if (!senderLayer.IsLowerLayer())
			return;

		float peakIntensity = targetIntensity + (float) (_hitTweenSetting.FadeIn?.Value?.Value ?? 0);

		_hitTween = CreateTween();

		_hitTweenSetting.FadeIn?.TweenProperty(_hitTween, this, peakIntensity);
		_hitTweenSetting.FadeOut?.TweenProperty(_hitTween, this, targetIntensity);
	}

	private float IntensityFromHealth(float currentHealth)
	{
		if (currentHealth > _healthThreshold)
			return 0;

		return _interpolationCurve.Sample(currentHealth/_healthThreshold);
	}

	private void SetIntensity()
	{
		_hitTween?.Kill();
		float health = _healthManager.TopHealthLayer.GetLowerLayer().CurrentHealth;
		Intensity = IntensityFromHealth(health) * _maxIntensity;
	}
}
