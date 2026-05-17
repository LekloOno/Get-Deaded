using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class E_EnemyBuilder : Resource
{
    [Export]
    private Dictionary<E_EnemyDifficulty, E_EnemySettings> _settingsByDifficulty
        = CreateDefaultSettings();
        
    [Export] private PackedScene _enemyBase;

    private E_EnemySettings _sharedSettings;

    public E_Enemy Build()
    {
        E_Enemy enemy = _enemyBase.Instantiate<E_Enemy>();

        if (_sharedSettings == null)
        {
            _sharedSettings = new();
            SetDifficulty(E_EnemyDifficulty.EASY);
        }

        enemy.SetSettings(_sharedSettings);
        return enemy;
    }

    public void SetDifficulty(E_EnemyDifficulty difficulty)
    {
        if (_settingsByDifficulty.TryGetValue(E_EnemyDifficulty.EASY, out E_EnemySettings settings))
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