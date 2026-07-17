using System;
using Godot;

public partial class UI_ActiveSector : Control
{
    [Export] private SC_GameManager _gameManager = null!;
    [Export] private SC_ParentSector _baseSector = null!;
    [Export] private float _edgeMargin = 30f;
    [Export] private float _maxScale = 2.5f;
    [Export] private float _minScale = 1f;
    [Export] private float _edgeMaxScaleBoost = 1.6f;
    [Export] private float _minOpacity = 0.2f;
    [Export] private float _maxOpacity = 0.8f;
    [Export] private float _centerMaxOpacityFade = 0.5f;
    [Export] private float _minDistance = 6f;
    [Export] private float _maxDistance = 60f;

    [Export] private ANIM_TweenSetting _positionTweenSetting = null!;
    private Tween? _positionTween;
    [Export] private ANIM_TweenSetting _blinkTweenSetting = null!;
    [Export] private int _blinks = 4;
    private Tween? _blinkTween;

    public Vector3 WorldPosition = new();
    private Vector2 _dir;
    private Vector2 _halfSize;
    private bool _onScreen;

    public override void _Ready()
    {
        Hide();
        SetProcess(false);

        _baseSector.SectorChanged += OnSectorChanged;
        _baseSector.Initialized += OnInitialized;
        _gameManager.ResetGame += OnStop;
    }

    private void OnStop()
    {
        Hide();
        SetProcess(false);
    }

    private void OnInitialized()
    {
        SC_LeafSector? leaf = _baseSector.ActiveLeafSector();
        if (leaf == null)
            return;

        WorldPosition = leaf.GlobalPosition;
        SetProcess(true);
        Show();
        StartBlinking();
    }

    private void OnSectorChanged(SC_LeafSector sector)
    {
        _positionTween?.Kill();
        _positionTween = CreateTween();
        _positionTweenSetting.TweenProperty(_positionTween, this, sector.GlobalPosition, "WorldPosition");

        StartBlinking();
    }

    private bool _blinking = false;
    private void StartBlinking()
    {
        _blinking = true;
        Color baseMod = EnemyColorSetting.Color;
        baseMod.A = _maxOpacity;

        bool baseIsWhitish = baseMod.R + baseMod.G + baseMod.B > 2.55f;
        Color blinkMod = baseIsWhitish ? Colors.Black : Colors.White;
        
        _blinkTween?.Kill();
        _blinkTween = CreateTween();
        
        _blinkTween.SetLoops(_blinks);
        _blinkTweenSetting.TweenProperty(_blinkTween, this, blinkMod, "modulate");
        _blinkTweenSetting.TweenProperty(_blinkTween, this, baseMod, "modulate");
        _blinkTween.Finished += StopBlink;
    }

    private void StopBlink()
    {
        _blinkTween?.Kill();
        _blinking = false;
    }

    public override void _Process(double delta)
    {
        UpdatePosition();

        float distanceRatio = DistanceFadeRatio();
        float edgeRatio = _onScreen
            ? Mathf.Clamp(Mathf.Max(Mathf.Abs(_dir.X) / _halfSize.X, Mathf.Abs(_dir.Y) / _halfSize.Y), 0f, 1f)
            : 1f;

        if (!_blinking)
            UpdateOpacity(distanceRatio, edgeRatio);

        UpdateScale(distanceRatio, edgeRatio);
    }

    private void UpdateOpacity(float distanceRatio, float edgeRatio)
    {
        Color mod = EnemyColorSetting.Color;
        mod.A = Mathf.Lerp(_minOpacity, _maxOpacity, distanceRatio);
        mod.A *= Mathf.Lerp(_centerMaxOpacityFade, 1f, edgeRatio);
        Modulate = mod;
    }

    private void UpdateScale(float distanceRatio, float edgeRatio)
    {
        float distanceScale = Mathf.Lerp(_minScale, _maxScale, distanceRatio);
        float edgeMultiplier = Mathf.Lerp(1f, _edgeMaxScaleBoost, edgeRatio);
        float scale = distanceScale * edgeMultiplier;
        Scale = new(scale, scale);
    }

    private void UpdatePosition()
    {
        Camera3D camera = GetViewport().GetCamera3D();
        if (camera == null)
            return;

        Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
        Vector2 screenCenter = viewportSize / 2f;

        bool behind = camera.IsPositionBehind(WorldPosition);
        Vector2 screenPos = camera.UnprojectPosition(WorldPosition);

        if (behind)
            screenPos = screenCenter + (screenCenter - screenPos);

        _dir = screenPos - screenCenter;
        _halfSize = screenCenter - new Vector2(_edgeMargin, _edgeMargin);

        _onScreen = !behind
            && screenPos.X >= _edgeMargin && screenPos.X <= viewportSize.X - _edgeMargin
            && screenPos.Y >= _edgeMargin && screenPos.Y <= viewportSize.Y - _edgeMargin;

        if (_onScreen)
        {
            base.Position = screenPos - Size / 2f;
            return;
        }

        if (_dir == Vector2.Zero)
            _dir = Vector2.Up;

        float scaleX = _dir.X != 0f ? _halfSize.X / Mathf.Abs(_dir.X) : float.PositiveInfinity;
        float scaleY = _dir.Y != 0f ? _halfSize.Y / Mathf.Abs(_dir.Y) : float.PositiveInfinity;
        float scale = Mathf.Min(scaleX, scaleY);

        base.Position = screenCenter + _dir * scale - Size / 2f;
    }

    private float DistanceFadeRatio()
    {
        if (_gameManager.Player == null)
            return 0f;

        float distance = _gameManager.Player.Body.GlobalTransform.Origin.DistanceTo(WorldPosition);
        
        if (distance <= _minDistance)
            return 0f;
        
        if (distance >= _maxDistance)
            return 1f;

        return (distance - _minDistance) / _maxDistance;
    }
}