using Godot;

public abstract partial class ANIM_TraumaLayer<T> : Resource
{
    [Export] private FastNoiseLite _noise;
    [Export] private float _reductionRate = 1f;
    [Export] private float _noiseSpeed = 50f;
    private bool _active = false;
    private float _trauma = 0f;

    protected abstract T Zero();
    protected abstract T ShakeVector(float intensity, float time);

    public T GetShakeAngleIntensity(float delta, float time)
    {
        if (!_active)
            return Zero();

        _trauma -= delta * _reductionRate;

        if (_trauma > 0)
        {
            float intensity = GetShakeIntensity();
            return ShakeVector(intensity, time);
        }

        _trauma = 0f;
        _active = false;
        return Zero();
    }


    protected float GetNoiseFromSeed(int seed, float time)
    {
        _noise.Seed = seed;
        return _noise.GetNoise1D(time * _noiseSpeed);
    }

    public float GetShakeIntensity() => _trauma * _trauma;

    public void AddTrauma(float amount)
    {
        _trauma = Mathf.Clamp(_trauma + amount, 0f, 1f);
        _active = true;
    }

    public void AddClampedTrauma(float amount, float max) =>
        AddTrauma(Mathf.Max(Mathf.Min(_trauma + amount, max) - _trauma, 0f));
}