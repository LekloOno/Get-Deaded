
using System;
using System.Linq;
using Shared.Scores;

public partial class STAT_ArenaGame
{
    public readonly DATA_Map MapData;
    public Difficulty Difficulty {get; private set;}
    public STAT_Combat CombatStat {get; private set;}
    public uint Score;
    public ulong Time;

    public STAT_ArenaGame() {}

    public STAT_ArenaGame(STAT_Combat combatStat)
    {
        MapData = new("dust_pit", "DUST_PIT");
        Difficulty = E_DifficultyServer.Difficulty;
        CombatStat = combatStat;
    }

    public void SetDifficulty(Difficulty difficulty) =>
        Difficulty = difficulty;

    public void Reset()
    {
        CombatStat.Reset();
        Difficulty = E_DifficultyServer.Difficulty;
        Score = 0;
        Time = 0;
    }

    public SubmitScoreRequest ToScoreReq()
    {
        return new SubmitScoreRequest(
            MapData.Id,
            "test",
            Difficulty,
            (int)Time,
            (int)Score,
            [
                CombatStat.MeleeWeapon.ToDto(),
                ..CombatStat.Weapons.Select(x => x.ToDto())
            ]
        );
    }
}