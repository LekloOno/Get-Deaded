using Godot;

[GlobalClass]
public partial class PickableSpawner : Node3D
{
    [Export] private GL_PickableData _pickableData;
    private GL_PhysicsPickable _current;
    /// <summary>
    /// Horizontal dampening of the pickup
    /// </summary>
    [Export] private float _horizontalDamp;
    /// <summary>
    /// Life time of the pickup
    /// </summary>
    [Export] private float _lifeTime = 0f;
    /// <summary>
    /// Determines whether or not the spawner spawns its pickup right on enabled
    /// Or if it first spawns it after its spawning delay.
    /// </summary>
    [Export] private bool _startSpawned;
    /// <summary>
    /// The time for the spawner to spawn the pickup once it has been picked up.
    /// </summary>
    [Export] private float _respawnDelay;
    /// <summary>
    /// Whether the spawner is enabled by entering a scene
    /// or being triggered by an external actor, like a scenario script.
    /// </summary>
    [Export] private bool _selfEnable = false;
    private SceneTreeTimer _respawnTimer;

    public override void _Ready()
    {
        SC_EntitiesManager.Register(this);

        if (_selfEnable)
            Enable();
    }

    public override void _ExitTree()
    {
        SC_EntitiesManager.Unregister(this);
    }

    public void Enable()
    {
        if (_startSpawned)
            Drop();
        else
            Respawn();
    }

    public void Disable()
    {
        if (_current != null)
        {
            _current.QueueFree();
            Clean();
        }

        if (_respawnTimer != null)
        {
            _respawnTimer.Timeout -= Drop;
            _respawnTimer = null;
        }
    }

    public void Drop()
    {
        if (_current != null)
            return;

        _current = _pickableData.GeneratePhysics(_horizontalDamp, _lifeTime);
        _current.TreeExiting += OnFree;
        AddChild(_current);
        _current.TopLevel = true;
        _current.GlobalPosition = GlobalPosition;
    }

    private void OnFree()
    {
        Clean();
        Respawn();
    }

    private void Respawn()
    {   
        _respawnTimer = GetTree().CreateTimer(_respawnDelay, false, true);
        _respawnTimer.Timeout += Drop;
    }

    private void Clean()
    {
        _current.TreeExiting -= OnFree;
        _current = null;
    }
}