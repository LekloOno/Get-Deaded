using System;
using Godot;

[GlobalClass]
public partial class UI_PickupCooldown : TextureProgressBar
{
    [Export] private PickableSpawner _pickupSpawner;
    [Export] private float _fadeInTime = 0.1f;
    [Export] private float _fadeOutTime = 0.2f;
    private Tween _loadTween;
    private Tween _fadeInTween;

    public override void _Ready()
    {
        _pickupSpawner.StartLoading += OnStartLoading;
        Color mod = Modulate;
        mod.A = 0f;
        Modulate = mod;
        Value = MinValue;

        SC_EntitiesManager.PickupsDisabled += OnPickupsDisabled;
    }

    private void OnPickupsDisabled()
    {
        _loadTween?.Kill();
        _fadeInTween?.Kill();

        _loadTween = CreateTween();
        _loadTween.TweenProperty(this, "modulate:a", 0f, _fadeOutTime);
    }


    private void OnStartLoading(float delay)
    {
        Value = MinValue;

        _loadTween?.Kill();
        _fadeInTween?.Kill();

        _loadTween = CreateTween();
        _fadeInTween = CreateTween();

        _fadeInTween.TweenProperty(this, "modulate:a", 1f, _fadeInTime);
        _loadTween.TweenProperty(this, "value", MaxValue, delay);
        _loadTween.TweenProperty(this, "modulate:a", 0f, _fadeOutTime);
    }
}