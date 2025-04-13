using Godot;

[GlobalClass]
public partial class PWF_Continuous : PW_Fire
{
    private SceneTreeTimer _timer;
    public override void Disable() => StopShoot();

    protected override bool SpecPress()
    {
        StopShoot();
        _recoil.ResetBuffer();

        if (!CanShoot())
            return false;
        
        Shoot();
        _recoil?.Start();

        _timer = GetTree().CreateTimer(_fireRate/1000f);
        _timer.Timeout += ReShoot;
        return true;
    }
    protected override bool SpecRelease()
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
        _timer = GetTree().CreateTimer(_fireRate/1000f);
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