using Godot;

namespace Pew;

[GlobalClass]
public partial class PC_DirectCamera : Camera3D
{
    public float _initialFov;
    private Tween _fovTween;

    public override void _Ready()
    {
        _initialFov = Fov;
    }

    public void ModifyFov(float modifier, float time)
    {
        _fovTween?.Kill();
        _fovTween = CreateTween();
        _fovTween.TweenProperty(this, "fov", _initialFov*modifier, time).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
    }

    public void ResetFov(float time)
    {
        _fovTween?.Kill();
        _fovTween = CreateTween();
        _fovTween.TweenProperty(this, "fov", _initialFov, time).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
    }
}