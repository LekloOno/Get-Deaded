using Godot;

[GlobalClass]
public partial class UIW_UnactiveFireHolder : UIW_FireHolder
{
    [Export] private Label _totalAmmos;
    public override void InnerInitialize(PW_Fire fire) => UpdateAmmos();
    protected override void HandleLoaded(int amount, uint finalAmount) => UpdateAmmos();
    protected override void HanldedUnloaded(int amount, uint finalAmount) => UpdateAmmos();
    private void UpdateAmmos() => _totalAmmos.Text = _ammos.TotalAmmos() + "";
}