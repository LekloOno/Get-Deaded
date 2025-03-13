using Godot;

[GlobalClass]
public partial class WE_Death : Node
{
    [Export] private PM_Controller _playerController;

    public override void _Ready()
    {
        _playerController.OnDie += (o, e) => _playerController.Revive();
    }
}