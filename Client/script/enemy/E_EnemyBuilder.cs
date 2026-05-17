using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class E_EnemyBuilder : Resource
{
    public E_EnemyBuilder()
    {
        E_DifficultyServer.Register(this);
    }

    [Export]
    private Dictionary<E_EnemyDifficulty, E_EnemySettings> _settingsByDifficulty
        = CreateDefaultSettings();
        
    [Export] private PackedScene _enemyBase;

    private E_EnemySettings _sharedSettings;
    private E_EnemyDifficulty _currentDifficulty;

    public E_Enemy Build()
    {
        if (_sharedSettings == null)
        {
            _sharedSettings = new();
            ForceDifficulty(E_EnemyDifficulty.EASY);
        }

        E_Enemy enemy = _enemyBase.Instantiate<E_Enemy>();

        enemy.SetSettings(_sharedSettings);
        return enemy;
    }

    public void SetDifficulty(E_EnemyDifficulty difficulty)
    {
        if (difficulty == _currentDifficulty)
            return;

        ForceDifficulty(difficulty);
    }

    private void ForceDifficulty(E_EnemyDifficulty difficulty)
    {
        if (!_settingsByDifficulty.TryGetValue(difficulty, out E_EnemySettings settings))
            return;

        _currentDifficulty = difficulty;
        _sharedSettings.UpdateFrom(settings);
    }

    private static Dictionary<E_EnemyDifficulty, E_EnemySettings> CreateDefaultSettings()
    {
        var dict = new Dictionary<E_EnemyDifficulty, E_EnemySettings>();

        foreach (E_EnemyDifficulty difficulty in Enum.GetValues<E_EnemyDifficulty>())
        {
            dict[difficulty] = null;
        }

        return dict;
    }
}