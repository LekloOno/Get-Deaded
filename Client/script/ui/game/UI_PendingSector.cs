using Godot;

public partial class UI_PendingSector : Control
{
    [Export] private SC_GameMode _gameManager = null!;
    [Export] private SC_ParentSector _baseSector = null!;
    [Export] private float _edgeMargin = 50f;
    [Export] private float _baseScale = 1f;
    [Export] private float _edgeMaxScaleBoost = 1.6f;
    [Export] private float _centerOpacity = 0.35f;

    [Export] private ANIM_TweenSetting _positionTweenSetting = null!;
    private Tween? _positionTween;

    public Vector3 WorldPosition = new();
    private Vector2 _dir;
    private Vector2 _halfSize;
    private bool _onScreen;

    public override void _Ready()
    {
        Hide();
        SetProcess(false);

        _baseSector.Initialized += OnInitialized;
        _baseSector.SectorChangedTo += OnSectorChanged;
        _gameManager.Reseted += OnStop;
    }

    private void OnStop()
    {
        _positionTween?.Kill();
        Hide();
        SetProcess(false);
    }

    private void OnInitialized()
    {
        SC_LeafSector? next = _baseSector.ActiveLeafSector()?.NextLeafSector();
        if (next == null)
            return;

        WorldPosition = next.GlobalPosition;
        SetProcess(true);
        Show();
    }

    private void OnSectorChanged(SC_LeafSector sector)
    {
        SC_LeafSector? next = sector.NextLeafSector();

        if (next == null)
        {
            _positionTween?.Kill();
            Hide();
            SetProcess(false);
            return;
        }

        if (!Visible)
        {
            WorldPosition = next.GlobalPosition;
            Show();
            SetProcess(true);
            return;
        }

        _positionTween?.Kill();
        _positionTween = CreateTween();
        _positionTweenSetting.TweenProperty(_positionTween, this, next.GlobalPosition, "WorldPosition");
    }

    public override void _Process(double delta)
    {
        UpdatePosition();

        float edgeRatio = _onScreen
            ? Mathf.Clamp(Mathf.Max(Mathf.Abs(_dir.X) / _halfSize.X, Mathf.Abs(_dir.Y) / _halfSize.Y), 0f, 1f)
            : 1f;

        UpdateScale(edgeRatio);
        UpdateOpacity(edgeRatio);
    }

    private void UpdateOpacity(float edgeRatio)
    {
        Color mod = SelfModulate;
        mod.A = Mathf.Lerp(_centerOpacity, 1f, edgeRatio);
        SelfModulate = mod;
    }

    private void UpdateScale(float edgeRatio)
    {
        float scale = _baseScale * Mathf.Lerp(1f, _edgeMaxScaleBoost, edgeRatio);
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
            Position = screenPos - Size / 2f;
            return;
        }

        if (_dir == Vector2.Zero)
            _dir = Vector2.Up;

        float scaleX = _dir.X != 0f ? _halfSize.X / Mathf.Abs(_dir.X) : float.PositiveInfinity;
        float scaleY = _dir.Y != 0f ? _halfSize.Y / Mathf.Abs(_dir.Y) : float.PositiveInfinity;
        float scale = Mathf.Min(scaleX, scaleY);

        Position = screenCenter + _dir * scale - Size / 2f;
    }
}