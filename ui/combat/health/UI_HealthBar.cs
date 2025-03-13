using Godot;

[GlobalClass]
public partial class UI_HealthBar : Control
{
    [Export] private ProgressBar _body;
    [Export] private ProgressBar _tail;
    [Export] private float _tailSpeed;

    private Tween _tailTween;
    private StyleBoxFlat _bodyColor;
    private StyleBoxFlat _tailColor;

    public void InitBar(float _maxHealth, float _initHealth, DATA_BarColors barColors)
    {
        _body.MinValue = _tail.MinValue = 0f;
        _body.MaxValue = _tail.MaxValue = _maxHealth;
        _body.Value = _tail.Value = _initHealth;

        _bodyColor = (StyleBoxFlat) _body.GetThemeStylebox("fill");
        _tailColor = (StyleBoxFlat) _tail.GetThemeStylebox("fill");
        
        SetColor(barColors);
    }

    public void SetColor(DATA_BarColors barColors)
    {
        _bodyColor.BgColor = barColors.Body;
        _tailColor.BgColor = barColors.Tail;
    }

    public void Break(DATA_BarColors barColors)
    {
        SetColor(barColors);
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