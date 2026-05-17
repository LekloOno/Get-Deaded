using Godot;

[GlobalClass]
public partial class PC_DirectCamera : Camera3D
{
    [Export] private PC_Settings _settings;
    private float _baseFov;

    private Tween _fovTween;

    public override void _Ready()
    {
        _settings.Fov.Subscribe(OnBaseFovChanged);
        _baseFov = _settings.Fov;
        Fov = _baseFov;
    }

    public void OnBaseFovChanged(float value)
    {
        if (Mathf.IsEqualApprox(Fov, _baseFov))
            Fov = value;
        _baseFov = value;
    }

    public void ModifyFov(float modifier, float time)
    {
        _fovTween?.Kill();
        _fovTween = CreateTween();
        _fovTween.TweenProperty(this, "fov", _baseFov * modifier, time)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
    }

    public void ResetFov(float time)
    {
        _fovTween?.Kill();
        _fovTween = CreateTween();
        _fovTween.TweenProperty(this, "fov", _baseFov, time)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
    }
}