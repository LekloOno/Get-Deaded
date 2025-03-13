using Godot;

[GlobalClass]
public partial class UI_HealthBar : Control
{
    [Export] private ProgressBar _body;
    [Export] private ProgressBar _tail;
    [Export] private float _tailSpeed;

    private Tween _tailTween;

    public void InitBar(float _maxHealth, float _initHealth)
    {
        _body.MinValue = _tail.MinValue = 0f;
        _body.MaxValue = _tail.MaxValue = _maxHealth;
        _body.Value = _tail.Value = _initHealth;
    }

    public void Damage(float currentHealth)
    {
        _body.Value = currentHealth;

        _tailTween?.Kill();
        _tailTween = CreateTween();
        _tailTween.TweenProperty(_tail, "value", _body.Value, _tailSpeed).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);
    }

    public void Heal(float currentHealth)
    {
        _body.Value = currentHealth;

        _tailTween?.Kill();

        _tail.Value = Mathf.Max(_tail.Value, _body.Value);

        _tailTween = CreateTween();
        _tailTween.TweenProperty(_tail, "value", _body.Value, _tailSpeed).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);
    }
}