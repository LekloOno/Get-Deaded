using System;
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
        _gameManager.ResetGame += Reset;
        AddChild(_player);
        RemoveChild(_player);
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    private void Reset()
    {
        _menuCamera.MakeCurrent();
        Input.MouseMode = Input.MouseModeEnum.Visible;
        RemoveChild(_player);
    }

    public void StartGame()
    {
        AddChild(_player);
        _player.Revive();
        _player.GlobalPosition = _spawnPoint.GlobalPosition;
        _player.VelocityCache.DiscardCache();
        _player.Velocity = Vector3.Zero;
        _player.Camera.MakeCurrent();
        Input.MouseMode = Input.MouseModeEnum.Captured;

        _gameManager.Init(_player);

    }
}