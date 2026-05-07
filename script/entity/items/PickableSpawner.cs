using Godot;

[GlobalClass]
public partial class PickableSpawner : Node3D
{
    [Export] private GL_PickableData _pickableData;
    private GL_PhysicsPickable _current;
    [Export] private float _horizontalDamp;
    [Export] private float _lifeTime = 0f;
    [Export] private bool _startSpawned;
    [Export] private float _respawnDelay;
    private SceneTreeTimer _respawnTimer;

    public override void _Ready()
    {
        if (_startSpawned)
            Drop();
        else
            Respawn();
    }

    public void Drop()
    {
        if (_current != null)
            return;

        _current = _pickableData.GeneratePhysics(_horizontalDamp, _lifeTime);
        _current.TreeExiting += OnFree;
        GetTree().Root.AddChild(_current);
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