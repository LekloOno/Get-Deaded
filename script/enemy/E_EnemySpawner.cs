using Godot;

[GlobalClass]
public partial class E_EnemySpawner : Node3D
{
    [Export] private E_Enemy _ennemy;
    [Export] private float _respawnDelay;
    private SceneTreeTimer _respawnTimer;
    private SceneTreeTimer _hideTimer;

    public override void _Ready()
    {
        _ennemy.OnDie += Die;
    }

    public void Die(E_IEnemy _, GC_Health senderLayer)
    {
        _respawnTimer = GetTree().CreateTimer(_respawnDelay, false, true);
        _respawnTimer.Timeout += Respawn;
    }

    public void Respawn()
    {
        _ennemy.GlobalPosition = GlobalPosition;
        _ennemy.Spawn();
    }
}