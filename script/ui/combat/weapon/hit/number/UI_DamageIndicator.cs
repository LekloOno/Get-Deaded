using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class UI_DamageIndicator : Label
{
    public float HeightOffset {get; set;} = 0f;
    [Export] private float _maxHeightOffset;
    [Export] private float _fadeTime;
    private Camera3D _camera;
    private Node3D _target;
    private float _damage;
    private Task _lastTweenTask = Task.CompletedTask;
    private Tween _opacityTween;
    private Tween _offsetTween;
    private Vector3 _prevTargetPosition;
    
    public void Initialize(Camera3D camera, Node3D target, float damage, Color color)
    {
        _camera = camera;
        _target = target;
        _prevTargetPosition = target.GlobalPosition;
        _damage = damage;
        Text = (int)_damage + "";
        AddThemeColorOverride("font_shadow_color", color);
        StartAnim();
    } 

    public void Stack(float damage, Color color)
    {
        _damage += damage;
        Text = (int)_damage + "";
        AddThemeColorOverride("font_shadow_color", color);
        _opacityTween?.Kill();
        _offsetTween?.Kill();
        StartAnim();
    }

    public void StartAnim()
    {
        _lastTweenTask = Anim();
    }

    public async Task Anim()
    {
        Color mod = Modulate;
        mod.A = 1f;
        Modulate = mod;

        HeightOffset = 0f;

        _opacityTween = CreateTween();
        _opacityTween.TweenProperty(this, "modulate:a", 0.0f, _fadeTime);

        _offsetTween = CreateTween();
        _offsetTween.TweenProperty(this, "HeightOffset", _maxHeightOffset, _fadeTime);

        await ToSignal(_opacityTween, "finished");

        QueueFree();
    }

    public override void _Process(double delta)
    {
        Vector3 worldPosition = new Vector3(0f, HeightOffset, 0f);

        if (_target != null && IsInstanceValid(_target) && _target.IsInsideTree())
        {
            worldPosition += _target.GlobalPosition;
            _prevTargetPosition = _target.GlobalPosition;
        }
        else
            worldPosition += _prevTargetPosition;


        Visible = !_camera.IsPositionBehind(worldPosition);
        Position = _camera.UnprojectPosition(worldPosition);
    }
}