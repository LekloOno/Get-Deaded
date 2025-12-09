using System.Reflection.Metadata.Ecma335;
using Godot;

[GlobalClass]
public partial class SC_AimArenaAmoConfig : Area3D
{
    [Export] private SC_AimArenaSpawner _game;
    [Export] private SC_GameManager _gameManager;
    bool _infActive = false;
    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("special_interaction"))
        {
            if (!TryGetPlayer(out PM_Controller player))
                return;

            _infActive = !_infActive;
            player.WeaponsHandler.SetInfiniteMagazine(_infActive);
            player.WeaponsHandler.SetInfiniteAmmo(_infActive);
        } else if (Input.IsActionJustPressed("start_game"))
        {
            if (!TryGetPlayer(out PM_Controller player))
                return;

            _gameManager.Init(player);
        }
    }

    private bool TryGetPlayer(out PM_Controller player)
    {
        foreach (var body in GetOverlappingBodies())
            if (body is PM_Controller p)
            {
                player = p;
                return true;
            }
        player = null;
        return false;
    }
}