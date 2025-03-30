using Godot;

[GlobalClass]
public partial class UI_WeaponHolder : BoxContainer
{
    [Export] private TextureRect _icon;
    [Export] private UIW_FireHolder _fireHolderTemplate;
    public void Initialize(PW_Weapon weapon)
    {
        if (_fireHolderTemplate.GetParent() == this)
            RemoveChild(_fireHolderTemplate);
        
        _icon.Texture = weapon.Icon;
        _icon.Modulate = weapon.IconColor;

        foreach (Node child in GetChildren())
            child.QueueFree();

        foreach (PW_Fire fire in weapon.GetFireModes())
        {
            if (fire.IsDerived)
                continue;

            UIW_FireHolder fireHolder = (UIW_FireHolder) _fireHolderTemplate.Duplicate();
            fireHolder.Initialize(fire);
            AddChild(fireHolder);
        }
    }
}