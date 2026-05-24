using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class UI_DamageIndicator : Label
{
    public float HeightOffset {get; set;} = 0f;
    [Export] private float _maxHeightOffset;
    [Export] private float _fadeTime;
    
    [Export] private float _maxScale = 2f;
    [Export] private float _maxScaleTime = 0.1f;
    [Export] private Tween.TransitionType _scaleInTrans;
    [Export] private float _minScale = 0.5f;
    [Export] private float _normalScaleTime = 0.1f;
    [Export] private Tween.TransitionType _scaleOutTrans;
    [Export] private ulong _chainLifetime = 150;
    [Export] private float _maxChainedDamage = 100f;
    [Export] private float _minScaleDelta = 0.15f;
    [Export] private float _horizontalOffsetLength = 0.5f;
    private float _chainedDamage;
    private Tween _scaleTween;

    private Camera3D _camera;
    private Node3D _target;
    private float _damage;
    private Task _lastTweenTask = Task.CompletedTask;
    private Tween _opacityTween;
    private Tween _offsetTween;
    private Vector3 _prevTargetPosition;

    private ulong _lastHit;
    
    public void Initialize(Camera3D camera, Node3D target, float damage, Color color, bool critical)
    {
        Scale = Vector2.Zero;
        _camera = camera;
        _target = target;
        _prevTargetPosition = target.GlobalPosition;
        _damage = damage;
        Text = (int)_damage + "";

        UpdateColor(color, critical);    
        
        StartAnim(damage);
    }

    private void UpdateColor(Color layerColor, bool critical)
    {
        LabelSettings.ShadowColor = layerColor;
        
        if (critical)
            LabelSettings.FontColor = Colors.Orange;
        else
            LabelSettings.FontColor = Colors.White;
    }

    public void Stack(float damage, Color color, bool critical)
    {
        _damage += damage;
        Text = (int)_damage + "";
        
        UpdateColor(color, critical);

        _opacityTween?.Kill();
        _offsetTween?.Kill();
        StartAnim(damage);
    }

    public void StartAnim(float damage)
    {
        DamageChainScale(damage, out float maxScale, out float minScale);

        _scaleTween?.Kill();
        _scaleTween = CreateTween();
        _scaleTween
            .TweenProperty(this, "scale", new Vector2(maxScale, maxScale), _maxScaleTime)
            .SetTrans(_scaleInTrans);

        _scaleTween
            .TweenProperty(this, "scale", new Vector2(minScale, minScale), _normalScaleTime)
            .SetTrans(_scaleOutTrans);

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
            Vector3 targetDisplayPos = _target.GlobalPosition;

            Vector3 horizontalOffset = ((_camera.GlobalPosition - _target.GlobalPosition)
                * new Vector3(1f, 0f, 1f))
                .Normalized()
                .Rotated(Vector3.Up, 0.5f * Mathf.Pi);

            targetDisplayPos += horizontalOffset * _horizontalOffsetLength;

            worldPosition += targetDisplayPos;
            _prevTargetPosition = targetDisplayPos;
        }
        else
            worldPosition += _prevTargetPosition;


        Visible = !_camera.IsPositionBehind(worldPosition);
        Position = _camera.UnprojectPosition(worldPosition);
    }

    private void DamageChainScale(float damage, out float peakScale, out float baseScale)
    {
        ulong now = PHX_Time.ScaledTicksMsec;
        if (now - _lastHit < _chainLifetime)
        {
            _chainedDamage += damage;
            peakScale = Mathf.Lerp(_maxScale, _minScale, Mathf.Min(_chainedDamage/_maxChainedDamage, 1));
            baseScale = Mathf.Min(_minScale, peakScale - _minScaleDelta);
            int scale = Mathf.RoundToInt(Mathf.Lerp(baseScale, peakScale, 0.5f));
            Scale = new Vector2(scale, scale);
        }
        else
        {
            _chainedDamage = 0;
            peakScale = _maxScale;
            baseScale = _minScale;   
        }

        _lastHit = now;
    } 
}