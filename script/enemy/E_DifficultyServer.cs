using System.Collections.Generic;

public static class E_DifficultyServer
{
    private static E_EnemyDifficulty _difficulty;
    public static E_EnemyDifficulty Difficulty
    {
        get => _difficulty;
        set
        {
            if (_difficulty == value)
                return;

            _difficulty = value;
            foreach (E_EnemyBuilder builder in _builders)
                builder.SetDifficulty(_difficulty);
        }
    }

    private static List<E_EnemyBuilder> _builders = [];

    public static void Register(E_EnemyBuilder builder) => _builders.Add(builder);
    public static void Unregister(E_EnemyBuilder builder) => _builders.Remove(builder);
}