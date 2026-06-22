using Godot;

[GlobalClass]
public partial class E_EnemySpawner : Node3D, E_IEnemyComponent
{
    public E_IEnemy? Enemy {get; set;}
    private NodePath _enemyPath = null!;
    [Export] public NodePath EnemyPath
    {
        get => _enemyPath;
        set => this.SetEnemy(this, ref _enemyPath, value);
    }

    [Export] private float _respawnDelay;
    private SceneTreeTimer? _respawnTimer;
    private SceneTreeTimer? _hideTimer;


    public override void _Ready()
    {
        (this as E_IEnemyComponent).ResolveEnemy(this);
    }

    public void Respawn()
    {
        Enemy!.Body.Teleport(GlobalPosition);
        Enemy.Spawn();
    }

    public void OnDied(E_IEnemy enemy, GC_Health senderLayer)
    {
        _respawnTimer = GetTree().CreateTimer(_respawnDelay, false, true);
        _respawnTimer.Timeout += Respawn;
    }
    public void OnPooled(E_IEnemy enemy) {}
    public void OnSpawned() {}
    public void OnDisabled(E_IEnemy enemy) {}
    public void OnEnemyChanged(E_IEnemy? prev, E_IEnemy? next) {}
}