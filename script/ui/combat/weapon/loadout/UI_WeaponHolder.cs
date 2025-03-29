using Godot;

[GlobalClass]
public partial class UI_WeaponHolder : AspectRatioContainer
{
    [Export] private TextureRect _icon;

    public void SetIcon(PW_Weapon weapon)
    {
        _icon.Texture = weapon.Icon;
        _icon.Modulate = weapon.IconColor;
    }
}