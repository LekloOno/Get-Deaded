using Godot;

namespace Pew;

[GlobalClass]
public partial class UIW_ActiveFireHolder : UIW_FireHolder
{
    [Export] private TextureRect _fireIcon;
    [Export] private Label _loadedAmmos;
    [Export] private Label _unloadedAmmos;
    public override void InnerInitialize(PW_Fire fire)
    {
        if (fire.Icon != null)
        {
            _fireIcon.Texture = fire.Icon;
            _fireIcon.Show();
        }
        else
            _fireIcon.Hide();

        _loadedAmmos.Text = _ammos.LoadedAmmos + "";
        _unloadedAmmos.Text = _ammos.UnloadedAmmos + "";
    }

    protected override void HandleLoaded(int amount, uint finalAmount) => _loadedAmmos.Text = finalAmount + "";
    protected override void HanldedUnloaded(int amount, uint finalAmount) => _unloadedAmmos.Text = finalAmount + "";
}