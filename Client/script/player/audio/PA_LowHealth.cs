using Godot;

public partial class PA_LowHealth : Node
{
    [Export] private GC_HealthManager _healthManager = null!;
    [Export] private AudioEffectLowPassFilter _filter = null!;
    [Export] private AudioEffectReverb _reverb = null!;
    [Export] private float _healthThreshold = 70f;
	[Export] private float _maxCutoff = 400f;
    [Export] private float _maxWet = 0.5f;
	[Export] private Curve _interpolationCurve = null!;
    [Export] private float _smoothSpeed;

    private float _intensity = 0f;
    private float _targetIntensity = 0f;
    public float Intensity
    {
        get => _intensity;
        set
        {
            if (value == _targetIntensity)
                SetPhysicsProcess(false);

            if (value == _intensity)
                return;

            _intensity = value;
            _filter.CutoffHz = Mathf.Lerp(20500f, _maxCutoff, value);
            _reverb.Wet = Mathf.Lerp(0, _maxWet, value);
        }
    }

    public float TargetIntensity
    {
        get => _targetIntensity;
        set
        {
            if (value != _intensity)
                SetPhysicsProcess(true);

            if (value == _targetIntensity)
                return;

            _targetIntensity = value;
        }
    }


    public override void _Ready()
    {
        _healthManager.OnDamage += OnDamage;
        _healthManager.OnHeal += OnHeal;
        _healthManager.OnLayerInit += OnInit;

        _filter.CutoffHz = 20500f;
        _reverb.Wet = 0f;
        SetPhysicsProcess(false);
    }

    public override void _PhysicsProcess(double delta)
    {
        Intensity = Mathf.Lerp(Intensity, TargetIntensity, (float) delta * _smoothSpeed);
    }

    private void OnInit(object? sender, HealthInitEventArgs e)
        => SetTarget();

    private void OnHeal(GC_Health senderLayer, DamageEventArgs e)
        => SetTarget();

    private void OnDamage(GC_Health senderLayer, DamageEventArgs e)
        => SetTarget();

    private void SetTarget()
    {
        TargetIntensity = IntensityManager(_healthManager);
    }

    private float IntensityManager(GC_HealthManager manager)
    {
        float health = manager.TopHealthLayer.TotalCurrent(out _);
		return IntensityFromHealth(health);
    }

    private float IntensityFromHealth(float currentHealth)
	{
		if (currentHealth > _healthThreshold)
			return 0;

		return _interpolationCurve.Sample(currentHealth/_healthThreshold);
	}
}