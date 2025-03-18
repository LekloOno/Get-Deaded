using Godot;

[GlobalClass]
public partial class UI_DamageIndicatorManager : Control
{
    [Export] private Camera3D _camera;
    [Export] private PackedScene _indicatorScene;
    [Export] private PW_WeaponsHandler _weaponsHandler;
    [Export] private Timer _bufferTimer;
    private UI_DamageIndicator _currentIndicator;

    public override void _Ready()
    {
        _weaponsHandler.Hit += HandleHit;
        _bufferTimer.OneShot = true;
        _bufferTimer.Timeout += ResetCurrentIndicator;
    }

    public void HandleHit(object sender, ShotHitEventArgs e)
    {
        if (e.HurtBox == null || e.Target == null)
            return;

        if (_currentIndicator != null)
        {
            _currentIndicator.Stack(e.Damage);
            _bufferTimer.Start();
        } else
        {
            Node indicatorNode = _indicatorScene.Instantiate();

            if(indicatorNode is UI_DamageIndicator indicator)
            {
                _currentIndicator = indicator;
                _currentIndicator.Initialize(_camera, e.Target, e.Damage);
                AddChild(_currentIndicator);
                _bufferTimer.Start();
            }
        }
    }

    public void ResetCurrentIndicator() => _currentIndicator = null;
}