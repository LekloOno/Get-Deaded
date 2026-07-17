using System;
using Godot;

public partial class UI_SectorChangedNotif : Control
{
    [Export] private SC_GameManager _gameManager = null!;
    [Export] private SC_ParentSector _baseSector = null!;
    [Export] private Vector2 _spawnOffset = new (0f, -400f);
    [Export] private Vector2 _initialScale = new (.65f, .65f);
    [Export] private float _worldInitialDistance = 5f;


    [Export] private ANIM_InOutTweenSetting _opacityTweenSetting = null!;
    private Tween? _opacityTween;
    [Export] private ANIM_InOutTweenSetting _scaleTweenSetting = null!;
    private Tween? _scaleTween;
    [Export] private ANIM_TweenSetting _positionTweenSetting = null!;
    [Export] private ANIM_TweenSetting _worldPositionTweenSetting = null!;
    private Tween? _positionTween;

    [Export] private float _holdTime = 1f;

    public Vector3 WorldPosition = new();
    private Vector2 _dir;
    private Vector2 _halfSize;
    private bool _onScreen;

    private Vector2 _initialPosition;

    public override void _Ready()
    {
        _initialPosition = Position;
        
        Color mod = EnemyColorSetting.Color;
        mod.A = SelfModulate.A;
        SelfModulate = mod;

        EnemyColorSetting.ValueChanged += OnColorChanged;

        Hide();
        SetProcess(false);

        _baseSector.SectorChangedTo += OnSectorChanged;
        _gameManager.ResetGame += OnStop;
    }

    private void OnColorChanged(GodotObject @object, Color color)
    {
        color.A = SelfModulate.A;
        SelfModulate = color;
    }

    private void OnStop()
    {
        _positionTween?.Kill();
        _opacityTween?.Kill();
        _scaleTween?.Kill();

        Hide();
        SetProcess(false);
    }

    private Vector3 _targetWorldPosition;
    private void OnSectorChanged(SC_LeafSector sector)
    {
        Show();
        _positionTween?.Kill();
        _opacityTween?.Kill();
        _scaleTween?.Kill();

        _positionTween = CreateTween();
        _opacityTween = CreateTween();
        _scaleTween = CreateTween();

        Position = _initialPosition + _spawnOffset;
        Scale = _initialScale;

        Color mod = Modulate;
        mod.A = 0f;
        Modulate = mod;
        
        _positionTweenSetting.TweenProperty(_positionTween, this, _initialPosition, "position");
        _opacityTweenSetting.FadeIn.TweenProperty(_opacityTween, this, 1f, "modulate:a");
        _scaleTweenSetting.FadeIn.TweenProperty(_scaleTween, this, Vector2.One, "scale");

        _opacityTween.TweenInterval(_holdTime);

        _targetWorldPosition = sector.GlobalPosition;
        _opacityTween.Finished += FadeOut;
    }

    private void FadeOut()
    {
        Camera3D camera = GetViewport().GetCamera3D();

        Vector3 origin = camera.ProjectRayOrigin(Position);
        Vector3 direction = camera.ProjectRayNormal(Position);

        WorldPosition = origin + direction * _worldInitialDistance;

        _positionTween?.Kill();
        _opacityTween?.Kill();
        _scaleTween?.Kill();

        _positionTween = CreateTween();
        _opacityTween = CreateTween();
        _scaleTween = CreateTween();

        _worldPositionTweenSetting.TweenProperty(_positionTween, this, _targetWorldPosition, "WorldPosition");
        _opacityTweenSetting.FadeOut.TweenProperty(_opacityTween, this, 0f, "modulate:a");
        _scaleTweenSetting.FadeOut.TweenProperty(_scaleTween, this, Vector2.Zero, "scale");

        _opacityTween.Finished += OnStop;

        SetProcess(true);
    }

    public override void _Process(double delta)
    {
        UpdatePosition();
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
        _halfSize = screenCenter;

        _onScreen = !behind;

        if (_onScreen)
        {
            Position = screenPos;
            return;
        }

        if (_dir == Vector2.Zero)
            _dir = Vector2.Up;

        float scaleX = _dir.X != 0f ? _halfSize.X / Mathf.Abs(_dir.X) : float.PositiveInfinity;
        float scaleY = _dir.Y != 0f ? _halfSize.Y / Mathf.Abs(_dir.Y) : float.PositiveInfinity;
        float scale = Mathf.Min(scaleX, scaleY);

        Position = screenCenter + _dir * scale;
    }
}