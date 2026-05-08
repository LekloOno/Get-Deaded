using Godot;

[GlobalClass]
public partial class SC_ArenaGameStarter : Node
{
    [Export] private Node3D _spawnPoint;
    [Export] private PackedScene _playerPrefab;
    [Export] private Camera3D _menuCamera;
    [Export] private SC_GameManager _gameManager;
    private PM_Controller _player;

    public override void _Ready()
    {
        _player = _playerPrefab.Instantiate<PM_Controller>();
    }

    public void StartGame()
    {
        GetTree().CurrentScene.AddChild(_player);
        _player.GlobalPosition = _spawnPoint.GlobalPosition;
        _player.Camera.MakeCurrent();

        _gameManager.Init(_player);
    }
}