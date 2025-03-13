using Godot;

[GlobalClass]
public partial class UI_HealthBar : Control
{
    [Export] private ProgressBar _health;
    [Export] private ProgressBar _tail;
    [Export] private float _tailSpeed;

    private Tween _tailTween;

    public void InitBar(float _maxHealth, float _initHealth)
    {
        _health.MinValue = _tail.MinValue = 0f;
        _health.MaxValue = _tail.MaxValue = _maxHealth;
        _health.Value = _tail.Value = _initHealth;
    }

    public void Damage(float currentHealth)
    {
        _health.Value = currentHealth;

        _tailTween?.Kill();
        _tailTween = CreateTween();
        _tailTween.TweenProperty(_tail, "value", _health.Value, _tailSpeed).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);
    }

    public void Heal(float currentHealth)
    {
        _health.Value = currentHealth;

        _tailTween?.Kill();

        _tail.Value = Mathf.Max(_tail.Value, _health.Value);

        _tailTween = CreateTween();
        _tailTween.TweenProperty(_tail, "value", _health.Value, _tailSpeed).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);
    }
}