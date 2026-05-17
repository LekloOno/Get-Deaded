using Godot;

[GlobalClass]
public partial class UIW_Weapon : BoxContainer
{
    [Export] private UIW_WeaponHolder _active;
    [Export] private UIW_WeaponHolder _unactive;
    [Export] private TextureRect _directBind;

    public bool Active {
        set {
            _unactive.Visible = !value;
            _directBind.Visible = !value;
            _active.Visible = value;
        }
    }
    
    public void Initialize(PW_Weapon weapon)
    {
        _active.Initialize(weapon);
        _unactive.Initialize(weapon);
    }

    public void SetActive()
    {
        _unactive.Hide();
        _directBind.Hide();
        _active.Show();
    }

    public void SetUnactive()
    {
        _active.Hide();
        _directBind.Show();
        _unactive.Show();
    }
}