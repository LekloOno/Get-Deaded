using System.Linq;

public class STAT_Weapon
{
    /*
    public class LocalHitsMerger
    {
        private STAT_Fire[] _fires;

        public LocalHitsMerger(STAT_Fire[] fires)
        {
            _fires = fires;
        }

        public int this[int i]
        {
            get => _fires.Sum(fire => fire.LocalHits.Value[i]);
        }

        public static explicit operator int[](LocalHitsMerger merger)
        {
            return Enumerable.Range(0, System.Enum.GetValues<GC_BodyPart>().Length)
                .Select(i => merger._fires.Sum(fire => fire.LocalHits.Value[i]))
                .ToArray();
        }
    }
    */
    public STAT_Fire[] Fires {get; private set;}

    public Observable<int> Shots {get; private set;} = new(0);
    public Observable<int> Hits {get; private set;} = new (0);
    public Observable<float> Damage {get; private set;} = new (0f);
    public Observable<int> Kills {get; private set;} = new (0);
    //public LocalHitsMerger LocalHits {get;}
    public Observable<int[]> LocalHits {get; private set;} = new(new int[System.Enum.GetValues<GC_BodyPart>().Length]);

    private void UpdateShots(int value) {Shots.Value = Fires.Sum(fire => fire.Shots);}
    private void UpdateHits(int value) {Hits.Value = Fires.Sum(fire => fire.Hits);}
    private void UpdateDamage(float value) {Damage.Value = Fires.Sum(fire => fire.Damage);}
    private void UpdateKills(int value) {Kills.Value = Fires.Sum(fire => fire.Kills);}
    private void UpdateLocalHits(int[] value)
    {
        LocalHits.Value = Enumerable.Range(0, System.Enum.GetValues<GC_BodyPart>().Length)
                .Select(i => Fires.Sum(fire => fire.LocalHits.Value[i]))
                .ToArray(); 
    }

    public STAT_Weapon(PW_Weapon weapon)
    {
        Fires = weapon.GetFireModes()
            .Select(fire => {
                STAT_Fire stat = new(fire, weapon.Icon, weapon.IconColor);
                stat.Shots.Subscribe(UpdateShots);
                stat.Hits.Subscribe(UpdateHits);
                stat.Damage.Subscribe(UpdateDamage);
                stat.Kills.Subscribe(UpdateKills);
                stat.LocalHits.Subscribe(UpdateLocalHits);
                return stat;
            })
            .ToArray();
    }

    public void Disable()
    {
        foreach (STAT_Fire fire in Fires)
            fire.Disable();
    }
}