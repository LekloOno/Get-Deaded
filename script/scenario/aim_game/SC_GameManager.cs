using Godot;

[GlobalClass]
public partial class SC_GameManager : Node
{
    [Export] private SC_SpawnerScript _initial;
    [Export] private float _countDown = 2f;
    [Signal] public delegate void InitializeEventHandler(SC_GameManager manager);
    public STAT_Combat Stats {get; private set;}
    private uint _score;
    public SceneTreeTimer CountDownTimer;
    private GE_CombatEntity _player;

    public void Init(GE_CombatEntity player)
    {
        _player = player;
        Stats = new(player);
        _score = 0;

        player?.WeaponsHandler.DisableFire();

        EmitSignal(SignalName.Initialize, this);

        CountDownTimer = GetTree().CreateTimer(_countDown);
        CountDownTimer.Timeout += StartGame;
    }

    private void StartGame()
    {
        CountDownTimer.Timeout -= StartGame;

        _player?.WeaponsHandler.EnableFire();
        _initial.Start();
    }

    public void HandleKill(E_IEnemy enemy, GC_Health senderLayer)
    {
        _score += enemy.Score;
    }

    public void EndGame()
    {
        Stats.Disable();
        GD.Print(_score);
    }
}