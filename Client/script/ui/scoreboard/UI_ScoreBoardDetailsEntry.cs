using Godot;
using Shared.Scores;

[GlobalClass]
public partial class UI_ScoreBoardDetailsEntry : Control
{
    [Export] private TextureRect _weapon;
    [Export] private Label _kills;
    [Export] private Label _damage;
    [Export] private Label _accuracy;
    [Export] private Label _critical;

    public void Initialize(WeaponStatDto weaponDetails)
    {
        _kills.Text = weaponDetails.Kills.ToString();
        _damage.Text = weaponDetails.Damage.ToString();

        //_weapon.Texture = //... need to implement a registry of weapons icons.

        _accuracy.Text = UI_ScoreBoardExtensions.DisplayAccuracy(weaponDetails.Accuracy);
        _critical.Text = UI_ScoreBoardExtensions.DisplayAccuracy(weaponDetails.CriticalAccuracy);
    }
}