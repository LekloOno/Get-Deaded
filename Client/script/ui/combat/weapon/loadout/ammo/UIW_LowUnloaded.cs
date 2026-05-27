using Godot;

[GlobalClass]
public partial class UIW_LowUnloaded : Control
{
    [Export] private Label? _label;
    [Export] private Label? _emptyLabel;

    [Export] private ANIM_InOutTweenSetting? _opacityTweenSettings;
    private Tween? _opacityTween;

    private bool _shown;

    public override void _Ready()
    {
        Color mod = Modulate;
        mod.A = 0f;
        Modulate = mod;
        _shown = false;
    }

    public void SetCritical()
    {
        _emptyLabel?.Hide();
        if (_label == null)
        {
            GD.PushError("[UIW_LowUnloaded] missing _label.");
            return;
        }
        
        _label.Show();
        _label.ThemeTypeVariation = "LowUnloadedCritical";
    }

    public void SetNormal()
    {
        _emptyLabel?.Hide();
        if (_label == null)
        {
            GD.PushError("[UIW_LowUnloaded] missing _label.");
            return;
        }
        
        _label.Show();
        _label.ThemeTypeVariation = "LowUnloaded";
    }

    public void SetEmpty()
    {
        _label?.Hide();
        if (_emptyLabel == null)
        {
            GD.PushError("[UIW_LowUnloaded] missing _emtpyLabel.");
            return;
        }
        _emptyLabel?.Show();
    }

    public void StartShow()
    {
        if (_shown)
            return;

        _shown = true;

        Color mod = Modulate;
        mod.A = 0f;
        Modulate = mod;
        
        _opacityTween?.Kill();
        _opacityTween = CreateTween();
        _opacityTweenSettings?.FadeIn?.TweenProperty(_opacityTween, this);
    }

    public void StartHide()
    {
        if (!_shown)
            return;

        _shown = false;

        _opacityTween?.Kill();
        _opacityTween = CreateTween();
        _opacityTweenSettings?.FadeOut?.TweenProperty(_opacityTween, this);
    }
}