using Godot;

[GlobalClass]
public partial class UI_PitScreen : Control
{
    [Export] private UIW_PlayerStat _playerStat = null!;

    public void InitializeStat(SC_ScoreManager manager)
    {
        _playerStat.Initialize(manager.GameStats.CombatStat, manager.Score);
    }
}