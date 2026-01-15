using Godot;

[GlobalClass]
public partial class PC_DirectCamera : Camera3D
{
    private float _baseFov;

    public float BaseFov
    {
        get => _baseFov;
        set
        {
            if (Mathf.IsEqualApprox(Fov, _baseFov))
                Fov = value;
            _baseFov = value;
        }
    }

    private Tween _fovTween;

    public override void _Ready()
    {
        _baseFov = Fov;
    }

    public void ModifyFov(float modifier, float time)
    {
        _fovTween?.Kill();
        _fovTween = CreateTween();
        _fovTween.TweenProperty(this, "fov", _baseFov*modifier, time).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
    }

    public void ResetFov(float time)
    {
        _fovTween?.Kill();
        _fovTween = CreateTween();
        _fovTween.TweenProperty(this, "fov", _baseFov, time).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
    }
}