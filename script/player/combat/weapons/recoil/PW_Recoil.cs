using Godot;

// Icon credits - Andy Meneely - under CC BY 3.0 - https://www.se.rit.edu/~andy/ - https://game-icons.net/1x1/andymeneely/riposte.html
[GlobalClass, Icon("res://gd_icons/weapon_system/recoil_icon.svg")]
public abstract partial class PW_Recoil : WeaponComponent 
{
    public MATH_AdditiveModifiers Modifier {get; protected set;} = new();
    public abstract void Initialize(PC_Recoil _recoilController);
    public abstract void Start();
    public abstract void Add();
    public abstract void Reset();
    public abstract void ResetBuffer();
}