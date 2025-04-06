using Godot;

[GlobalClass]
public partial class PC_Shakeable : Area3D
{
    [Export] private float _reductionRate = 1f;
    [Export] private FastNoiseLite _noise;
    [Export] private float _noiseSpeed = 50f;
    [Export] private Vector3 _maxRotation = new(10f, 10f, 5f);

    private float _trauma = 0f;
    private float _time = 0f;

    public override void _Ready()
    {
        SetProcess(false);
        CollisionLayer = CONF_Collision.Layers.Trauma;
        CollisionMask = 0;
    }

    public override void _Process(double delta)
    {
        _time += (float)delta;
        _trauma -= (float)delta * _reductionRate;

        if (_trauma > 0)
        {
            float intensity = GetShakeIntensity();
            Vector3 _shakeAngleIntensity = new(
                intensity * GetNoiseFromSeed(0),
                intensity * GetNoiseFromSeed(1),
                intensity * GetNoiseFromSeed(2)
                );
            
            RotationDegrees = _maxRotation * _shakeAngleIntensity;
        }
        else
        {
            _trauma = 0f;
            SetProcess(false);
        }
    }

    public void AddTrauma(float amount)
    {
        _trauma = Mathf.Clamp(_trauma + amount, 0f, 1f);
        SetProcess(true);
    }

    public float GetShakeIntensity() => _trauma * _trauma;
    public float GetNoiseFromSeed(int seed)
    {
        _noise.Seed = seed;
        return _noise.GetNoise1D(_time * _noiseSpeed);
    }
}