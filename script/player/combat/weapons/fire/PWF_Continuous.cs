using Godot;

[GlobalClass]
public partial class PWF_Continuous : PW_Fire
{
    private SceneTreeTimer _timer;
    public override void Disable() => StopShoot();


    protected override bool Press()
    {
        StopShoot();
        _recoil.ResetBuffer();        
        float nextShot = _fireRate;

        if (TryShoot())
        {
            _recoil?.Start();
        }
        else
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
        if (!_ammos.DidConsume(_ammosPerShot))
        {
            StopShoot();
            return;
        }

        Shoot();
        _recoil?.Add();
        _timer = _sight.GetTree().CreateTimer(_fireRate/1000f);
        _timer.Timeout += ReShoot;
    }

    private void StopShoot()
    {
        if (_timer != null)
        {
            _timer.Timeout -= ReShoot;
            _timer = null;
            _recoil?.Reset();
        }

    }
}