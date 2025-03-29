using Godot;

[GlobalClass]
public partial class UI_WeaponHolder : HBoxContainer
{
    [Export] private TextureRect _icon;
    [Export] private TextureRect _bind;

    public void SetIcon(PW_Weapon weapon)
    {
        _icon.Texture = weapon.Icon;
        _icon.Modulate = weapon.IconColor;
    }

    public void SetBind(){}
}