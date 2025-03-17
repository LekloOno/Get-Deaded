using Godot;

[GlobalClass]
public partial class PW_Alternate : PW_Weapon
{
    [Export] private PW_Fire _primaryFire;
    [Export] private PW_Fire _secondaryFire;
    [Export] private PW_AlternateADS _ads;
    private PW_Fire _currentFire;


    public override void WeaponInitialize()
    {
        _currentFire = _primaryFire;
        _primaryFire.Initialize(_camera, _sight);
        _secondaryFire.Initialize(_camera, _sight);

        if (_ads != null)
        {
            _ads?.Initialize(_camera);
            _ads?.AlternateInitialize(_primaryFire, _secondaryFire);
        }
    }

    public override void PrimaryDown() => _currentFire.Press();
    public override void PrimaryUp() => _currentFire.Release();

    public override void SecondaryDown()
    {
        if (_ads == null)
            _secondaryFire.Press();
        else if (_ads.ActivatedPress() is PW_Fire fire)
            _currentFire = fire;
    }

    public override void SecondaryUp()
    {
        if (_ads == null)
            _secondaryFire.Release();
        else if (_ads.ActivatedPress() is PW_Fire fire)
            _currentFire = fire;
    }
}