using Godot;

[GlobalClass]
public partial class E_EnnemySpawner : Node3D
{
    [Export] private E_Ennemy _ennemy;
    [Export] private float _respawnDelay;
    private SceneTreeTimer _respawnTimer;
    private SceneTreeTimer _hideTimer;

    public override void _Ready()
    {
        _ennemy.OnDie += Die;
    }

    public void Die(GC_Health senderLayer)
    {
        _ennemy.Disable();
        _respawnTimer = GetTree().CreateTimer(_respawnDelay);
        _respawnTimer.Timeout += Respawn;
    }

    public void Respawn()
    {
        _ennemy.GlobalPosition = GlobalPosition;
        _ennemy.Enable();
    }
}