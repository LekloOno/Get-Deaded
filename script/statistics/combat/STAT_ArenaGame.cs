
using System;

public partial class STAT_ArenaGame: IDisposable
{
    public E_EnemyDifficulty Difficulty {get; private set;}
    public STAT_Combat CombatStat {get; private set;}
    public uint Score {get; private set;}
    public ulong Time {get; private set;}

    private SC_GameManager _gameManager;
    private ulong _startTime;

    public STAT_ArenaGame() {}

    public STAT_ArenaGame(SC_GameManager gameManager, STAT_Combat combatStat)
    {
        Difficulty = E_DifficultyServer.Difficulty;

        _gameManager = gameManager;

        _gameManager.Score.Subscribe(UpdateScore);
        CombatStat = combatStat;
        _gameManager.StartGame += StartTimer;
        _gameManager.ResetGame += StopTimer;
    }

    private void StartTimer() =>
        _startTime = PHX_Time.ScaledTicksMsec;

    private void StopTimer() =>
        Time = PHX_Time.ScaledTicksMsec - _startTime;

    public void SetDifficulty(E_EnemyDifficulty difficulty) =>
        Difficulty = difficulty;

    private void UpdateScore(uint score) =>
        Score = score;

    public void Dispose()
    {
        _gameManager.Score.Unsubscribe(UpdateScore);
        _gameManager.StartGame -= StartTimer;
        _gameManager.ResetGame -= StopTimer;
    }

    public void Reset()
    {
        CombatStat.Reset();
        Difficulty = E_DifficultyServer.Difficulty;
        Score = 0;
        Time = 0;
    }
}