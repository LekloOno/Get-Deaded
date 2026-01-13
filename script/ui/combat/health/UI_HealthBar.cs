using Godot;

[GlobalClass]
public partial class UI_HealthBar : Control
{
    [Export] private ProgressBar _body;
    [Export] private ProgressBar _tail;
    [Export] private float _tailSpeed;
    [Export] private Tween.TransitionType _tailAnimation;

    private Tween _tailTween;
    private StyleBoxFlat _bodyColor;
    private StyleBoxFlat _tailColor;
    private CONFD_IBarColors _barColors;

    public void InitBar(float _maxHealth, float _initHealth, CONFD_IBarColors barColors)
    {
        _body.MinValue = _tail.MinValue = 0f;
        _body.MaxValue = _tail.MaxValue = _maxHealth;
        _body.Value = _tail.Value = _initHealth;

        _bodyColor = (StyleBoxFlat) _body.GetThemeStylebox("fill");
        _tailColor = (StyleBoxFlat) _tail.GetThemeStylebox("fill");
        
        _barColors = barColors;
        SetColor();
    }

    public void SetColor()
    {
        _bodyColor.BgColor = _barColors.Body;
        _tailColor.BgColor = _barColors.Tail;
    }

    public void Break(CONFD_IBarColors barColors)
    {
        _barColors = barColors;
        SetColor();
    }

    public void Damage(float currentHealth)
    {
        SetColor();
        _body.Value = currentHealth;

        _tailTween?.Kill();
        _tailTween = CreateTween();
        _tailTween.TweenProperty(_tail, "value", _body.Value, _tailSpeed).SetTrans(_tailAnimation);
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