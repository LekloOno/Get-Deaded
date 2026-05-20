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

    [Signal]
    public delegate void SceneInitializedEventHandler();
    
    [Signal]
    public delegate void RoamingStartedEventHandler();

    
    [Signal]
    public delegate void ResetedEventHandler();

    private bool _started;

    public override void _Ready()
    {
        _player = _playerPrefab.Instantiate<PM_Controller>();
        _gameManager.ResetGame += Reset;
        AddChild(_player);
        RemoveChild(_player);
        Input.MouseMode = Input.MouseModeEnum.Visible;
        SC_EntitiesManager.ForceSpawn();
    }

    public void Reset()
    {
        if (!_started)
            return;

        SC_EntitiesManager.ForceSpawn();
        _menuCamera.MakeCurrent();
        Input.MouseMode = Input.MouseModeEnum.Visible;
        RemoveChild(_player);

        _started = false;
        EmitSignal(SignalName.Reseted);
    }

    public void StartGame()
    {
        if (!InitScene())
            return;

        _gameManager.Init(_player);
    }

    public void StartRoaming()
    {
        if (!InitScene())
            return;

        SC_EntitiesManager.EnablePickups();

        EmitSignal(SignalName.RoamingStarted);
    }

    // Returns if it did initialized or not
    private bool InitScene()
    {
        if (_started)
            return false;

        _started = true;

        SC_EntitiesManager.DisablePickups();

        AddChild(_player);
        _player.Revive();
        _player.GlobalPosition = _spawnPoint.GlobalPosition;
        _player.ResetPhysicsInterpolation();
        _player.VelocityCache.DiscardCache();
        _player.Velocity = Vector3.Zero;
        _player.RealVelocity = Vector3.Zero;
        _player.Camera.MakeCurrent();

        EmitSignal(SignalName.SceneInitialized);
        Input.MouseMode = Input.MouseModeEnum.Captured;

        return true;
    }
}