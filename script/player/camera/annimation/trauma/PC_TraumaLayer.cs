using Godot;

[GlobalClass]
public partial class PC_TraumaLayer : Resource
{
    [Export] private FastNoiseLite _noise;
    [Export] private float _reductionRate = 1f;
    [Export] private float _noiseSpeed = 50f;
    private bool _active = false;
    private float _trauma = 0f;

    public Vector3 GetShakeAngleIntensity(float delta, float time)
    {
        if (!_active)
            return Vector3.Zero;

        _trauma -= delta;

        if (_trauma > 0)
        {
            float intensity = GetShakeIntensity();
            Vector3 _shakeAngleIntensity = new(
                intensity * GetNoiseFromSeed(0, time),
                intensity * GetNoiseFromSeed(1, time),
                intensity * GetNoiseFromSeed(2, time)
                );
            
            return _shakeAngleIntensity;
        }

        _trauma = 0f;
        _active = false;
        return Vector3.Zero;
    }

    private float GetNoiseFromSeed(int seed, float time)
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