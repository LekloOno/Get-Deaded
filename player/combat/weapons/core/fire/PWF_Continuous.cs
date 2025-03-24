using Godot;

[GlobalClass]
public partial class PWF_Continuous : PW_Fire
{
    private SceneTreeTimer _timer;
    private PC_RecoilHandler _recoilHandler;

    public override void Disable() => StopShoot();


    protected override bool Press()
    {
        StopShoot();

        _recoilController.ResetBuffer();
        //_recoilHandler = new(new(0.1f, 0.02f), 0.2f, false);

        
        float nextShot = _fireRate;

        if (TryShoot())
        {
            _recoilController.AddRecoil(new(0.05f, 0.6f), 0.1f);
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
        _recoilController.ResetRecoil(0.3f);
        return true;
    }

    private void ReShoot()
    {
        Shoot();
        _recoilController.AddRecoil(new(0.05f, 0.6f), 0.1f);
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