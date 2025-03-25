using Godot;

[GlobalClass]
public partial class PW_Alternate : PW_Weapon
{
    [Export] private PW_Fire _primaryFire;
    [Export] private PW_Fire _secondaryFire;
    private PW_Fire _currentFire;


    protected override void WeaponInitialize(PC_Recoil recoilController)
    {
        _currentFire = _primaryFire;
        _primaryFire.Initialize(_camera, _sight, _barel, recoilController);
        _secondaryFire.Initialize(_camera, _sight, _barel, recoilController);
        _primaryFire.Hit += (o, e) => Hit?.Invoke(o, e);
        _secondaryFire.Hit += (o, e) => Hit?.Invoke(o, e);
    }

    protected override void Disable() => _currentFire.Disable();
    protected override void PrimaryDown() => _currentFire.HandlePress();
    protected override void PrimaryUp() => _currentFire.HandleRelease();
    protected override void SecondaryDown() => _secondaryFire.HandlePress();
    protected override void SecondaryUp() => _secondaryFire.HandleRelease();

    protected override void StartADS()
    {
        _currentFire.Disable();
        _currentFire = _secondaryFire;
    }

    protected override void StopADS()
    {
        _currentFire.Disable();
        _currentFire = _primaryFire;
    }

    protected override void Reload()
    {
        _primaryFire.Reload();
        _secondaryFire.Reload();
    }

}