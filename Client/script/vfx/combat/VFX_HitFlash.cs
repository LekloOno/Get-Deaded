using System;
using Godot;

public partial class VFX_HitFlash : MeshInstance3D, PHX_PoolObject
{
    private GC_HurtBox? _hurtBox;

    [Export] private float _decayTime = 2.0f;
    [Export] private float _maxIntensity = 50f;
    [Export] private float _minIntensity = 8f;
    [Export] private float _maxDamage = 100f;
    [Export] private Tween.TransitionType _decayTrans;
    [Export] private Tween.EaseType _decayEase;
    [Export] private float _flashWhiteRatio = 0.1f;
    [Export] private float _maxScale = 0.4f;
    [Export] private float _minScale = 0.1f;
    [Export] private float _maxChainedDamage = 200f;

    private float _intensity = 1.0f;

    private float _chainedDamage;

    private Tween? _decayTween;

    public event Action? Spawned;

    public override void _Ready()
    {
        Pool();

        if (GetParent() is not GC_HurtBox hurtBox)
            return;

        _hurtBox = hurtBox;
        _hurtBox.HitReceived += OnHitReceived;
    }

    public void Initialize(Vector3 from, Vector3 to, HitEventArgs args)
    {
        float ratio = Mathf.Clamp(args.TotalDamage/_maxDamage, 0f, 1f);
        _intensity = Mathf.Lerp(_minIntensity, _maxIntensity, ratio);

        Color flashColor = CONF_HealthColors
            .GetBarColors(args.SenderLayer).Body
            .Lerp(Colors.White, _flashWhiteRatio);

        SetInstanceShaderParameter("flash_color", flashColor);

        Vector3 hitNormal = (from - to).Normalized();
        Vector3 right = hitNormal.Cross(Vector3.Up).Normalized();

        if (right.LengthSquared() < 0.001f)
            right = hitNormal.Cross(Vector3.Right).Normalized();

        Vector3 up = right.Cross(hitNormal).Normalized();

        GlobalTransform = new Transform3D(
            new Basis(right, up, hitNormal),
            to + hitNormal * 0.02f
        );
    }

    public override void _ExitTree()
    {
        if (_hurtBox != null)
            _hurtBox.HitReceived -= OnHitReceived;
    }

    private void OnHitReceived(Vector3 from, Vector3 to, HitEventArgs args)
    {
        Spawn();
        Initialize(from, to, args);

        Visible = true;
            
        UpdateSize(args.TotalDamage);

        _decayTween?.Kill();
        _decayTween = CreateTween();

        _decayTween
            .TweenMethod(
                Callable.From((float v) => SetInstanceShaderParameter("intensity", v)),
                _intensity,
                0.0f,
                _decayTime)
            .SetTrans(_decayTrans)
            .SetEase(_decayEase);

        _decayTween.TweenCallback(Callable.From(Pool));
    }

    private void UpdateSize(float damage)
    {
        if (Mesh is not QuadMesh quad)
            return;

        if (_decayTween != null && _decayTween.IsRunning())
            _chainedDamage += damage;
        else
            _chainedDamage = damage;

        float scaleRatio = Mathf.Clamp(_chainedDamage/_maxChainedDamage, 0f, 1f);
        float scale = Mathf.Lerp(_minScale, _maxScale, scaleRatio);

        quad.Size = Vector2.One * scale;
    }

    public void Pool()
    {
        Visible = false;
    }

    public void Spawn()
    {
        Visible = true;
        Spawned?.Invoke();
    }
}