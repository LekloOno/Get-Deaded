using Godot;

public partial class PWF_Continuous : PW_Fire
{
    private SceneTreeTimer _timer;
    public override void Press()
    {
        float nextShot = _fireRate;
        if (CanShoot())
            Shoot();
        else
            nextShot = _fireRate + _lastShot - Time.GetTicksMsec();

        _timer = GetLocalScene().GetTree().CreateTimer(nextShot/1000f);
        _timer.Timeout += ReShoot;
    }
    public override void Release()
    {
        StopShoot();
    }

    private void ReShoot()
    {
        Shoot();
        _timer = GetLocalScene().GetTree().CreateTimer(_fireRate/1000f);
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