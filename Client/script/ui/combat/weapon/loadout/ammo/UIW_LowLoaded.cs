using System;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class UIW_LowLoaded : Control
{
    [Export] private Label? _label;
    // Later use a custom displayable texture rect probably.
    [Export] private Label? _key;
    
    [Export] private ANIM_InOutTweenSetting? _opacityTweenSettings;
    private Tween? _opacityTween;
    [Export] private ANIM_InOutTweenSetting? _scaleTweenSettings;
    [Export] private Vector2 _startScale;
    private Tween? _scaleTween;

    private KeyBindingSetting? _bind;

    private bool _shown;

    public override void _Ready()
    {
        _bind = KeyBindingSettingsManager.Instance.GetBinding(ACTIONS_Combat.RELOAD);
        
        OnBindChanged(this, 0);

        _bind.Changed += OnBindChanged;

        Color mod = Modulate;
        mod.A = 0f;
        Modulate = mod;
    }

    private void OnBindChanged(GodotObject sender, Variant value)
    {
        if (_bind == null)
            return;

        if (_key == null)
            return;

        EditableInputEvent input;
        
        if (!_bind.TryGetBind(0, out input) &&
            !_bind.TryGetBind(1, out input) &&
            !_bind.TryGetBind(2, out input))
            return;

        InputEvent inputEvent = input.InputEvent;

        if (inputEvent is InputEventMouseButton mouseButton)
            _key.Text = "Mouse " + mouseButton.ButtonIndex;
        else if (inputEvent is InputEventKey keyButton)
            _key.Text = OS.GetKeycodeString(
                DisplayServer.KeyboardGetLabelFromPhysical(keyButton.PhysicalKeycode)
            );
    }

    public void StartShow()
    {
        if (_shown)
            return;

        _shown = true;

        Color mod = Modulate;
        mod.A = 0f;
        Modulate = mod;

        Scale = _startScale;
        
        _opacityTween?.Kill();
        _opacityTween = CreateTween();
        _opacityTweenSettings?.FadeIn?.TweenProperty(_opacityTween, this);

        _scaleTween?.Kill();
        _scaleTween = CreateTween();
        _scaleTweenSettings?.FadeIn?.TweenProperty(_scaleTween, this);
    }

    public void StartHide()
    {
        if (!_shown)
            return;

        _shown = false;

        _opacityTween?.Kill();
        _opacityTween = CreateTween();
        _opacityTweenSettings?.FadeOut?.TweenProperty(_opacityTween, this);

        _scaleTween?.Kill();
        _scaleTween = CreateTween();
        _scaleTweenSettings?.FadeIn?.TweenProperty(_scaleTween, this);
    }
}