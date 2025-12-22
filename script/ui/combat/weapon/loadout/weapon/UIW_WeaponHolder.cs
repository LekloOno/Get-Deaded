using System;
using System.Collections.Generic;
using Godot;

namespace Pew;

[GlobalClass]
public partial class UIW_WeaponHolder : BoxContainer
{
    [Export] private TextureRect _icon;
    [Export] private Container _fireHoldersContainer;
    [Export] private UIW_FireHolder _fireHolderTemplate;
    private List<UIW_FireHolder> _fireHolders = [];

    public void Initialize(PW_Weapon weapon)
    {
        // Clear previous state
        if (_fireHolderTemplate.GetParent() == _fireHoldersContainer)
            _fireHoldersContainer.RemoveChild(_fireHolderTemplate);

        foreach (UIW_FireHolder _fireHolder in _fireHolders)
            _fireHolder.QueueFree();
        
        _fireHolders.Clear();


        // Initialize new state
        _icon.Texture = weapon.Icon;
        _icon.Modulate = weapon.IconColor;

        foreach (PW_Fire fire in weapon.GetFireModes())
        {
            if (fire.IsDerived)
                continue;

            UIW_FireHolder fireHolder = (UIW_FireHolder) _fireHolderTemplate.Duplicate();
            fireHolder.Initialize(fire);
            _fireHolders.Add(fireHolder);

            _fireHoldersContainer.AddChild(fireHolder);
        }
    }
}