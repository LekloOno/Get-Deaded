using Godot;

public partial class UIW_ArenaEnd : Node
{
    [Export] private UI_EscapeMenu _escapeMenu;
    [Export] private Control _combatStats;
    [Export] private PI_Stats _statsInput;


    public override void _Ready()
    {
        _statsInput.Start += (o, e) => _combatStats.Show();
        _statsInput.Stop += (o, e) => _combatStats.Hide();
    }

    public void ShowScores()
    {
        _escapeMenu.Enter(_combatStats);
    }
}