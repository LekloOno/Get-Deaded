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
    private Task lastTweenTask = Task.CompletedTask;
    private Tween opacityTween;
    private Tween offsetTween;
    
    public void Initialize(Camera3D camera, Node3D target, float damage, Color color)
    {
        _camera = camera;
        _target = target;
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
        opacityTween?.Kill();
        offsetTween?.Kill();
        StartAnim();
    }

    public void StartAnim()
    {
        lastTweenTask = Anim();
    }

    public async Task Anim()
    {
        Color mod = Modulate;
        mod.A = 1f;
        Modulate = mod;

        HeightOffset = 0f;

        opacityTween = CreateTween();
        opacityTween.TweenProperty(this, "modulate:a", 0.0f, _fadeTime);

        offsetTween = CreateTween();
        offsetTween.TweenProperty(this, "HeightOffset", _maxHeightOffset, _fadeTime);

        await ToSignal(opacityTween, "finished");

        QueueFree();
    }

    public override void _Process(double delta)
    {
        Vector3 worldPosition = _target.GlobalPosition + new Vector3(0f, HeightOffset, 0f);
        Visible = !_camera.IsPositionBehind(worldPosition);
        Position = _camera.UnprojectPosition(worldPosition);
    }
}