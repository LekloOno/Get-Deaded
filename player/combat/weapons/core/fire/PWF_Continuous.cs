using Godot;

[GlobalClass]
public partial class PWF_Continuous : PW_Fire
{
    private SceneTreeTimer _timer;

    public override void Disable() => StopShoot();


    protected override bool Press()
    {
        StopShoot();
        
        float nextShot = _fireRate;

        if (!TryShoot())
            nextShot = NextAvailableShot();

        _timer = _sight.GetTree().CreateTimer(nextShot/1000f);
        _timer.Timeout += ReShoot;
        return true;
    }
    protected override bool Release()
    {
        StopShoot();
        return true;
    }

    private void ReShoot()
    {
        Shoot();
        _timer = _sight.GetTree().CreateTimer(_fireRate/1000f);
        _timer.Timeout += ReShoot;
    }

    private void StopShoot()
    {
        if (_timer != null)
        {
            _timer.Timeout -= ReShoot;
            _timer = null;
        }
    }
}