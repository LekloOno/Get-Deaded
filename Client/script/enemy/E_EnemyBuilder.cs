using System;
using Godot;
using Godot.Collections;
using Shared.Scores;

[GlobalClass]
public partial class E_EnemyBuilder : Resource
{
    public E_EnemyBuilder()
    {
        E_DifficultyServer.Register(this);
    }

    [Export]
    private Dictionary<Difficulty, E_EnemySettings> _settingsByDifficulty
        = CreateDefaultSettings();
        
    [Export] private PackedScene _enemyBase;

    private E_EnemySettings _sharedSettings;
    private Difficulty _currentDifficulty;

    public E_Enemy Build()
    {
        if (_sharedSettings == null)
        {
            _sharedSettings = new();
            ForceDifficulty(Difficulty.EASY);
        }

        E_Enemy enemy = _enemyBase.Instantiate<E_Enemy>();

        enemy.SetSettings(_sharedSettings);
        return enemy;
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        if (difficulty == _currentDifficulty)
            return;

        ForceDifficulty(difficulty);
    }

    private void ForceDifficulty(Difficulty difficulty)
    {
        if (!_settingsByDifficulty.TryGetValue(difficulty, out E_EnemySettings settings))
            return;

        _currentDifficulty = difficulty;
        _sharedSettings.UpdateFrom(settings);
    }

    private static Dictionary<Difficulty, E_EnemySettings> CreateDefaultSettings()
    {
        var dict = new Dictionary<Difficulty, E_EnemySettings>();

        foreach (Difficulty difficulty in Enum.GetValues<Difficulty>())
        {
            dict[difficulty] = null;
        }

        return dict;
    }
}