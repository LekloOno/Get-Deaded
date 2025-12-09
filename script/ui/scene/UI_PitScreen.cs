using Godot;

[GlobalClass]
public partial class UI_PitScreen : Control
{
    [Export] private UIW_PlayerStat _playerStat;

    public void InitializeStat(SC_GameManager manager)
    {
        _playerStat.Initialize(manager.Stats);
    }
}