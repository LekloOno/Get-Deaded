using System;
using System.Linq;
using Godot;

public class STAT_Weapon: IDisposable
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
    public DATA_Weapon WeaponData {get; private set;}
    public STAT_Fire[] Fires {get; private set;}

    public Observable<int> Shots {get; private set;} = new(0);
    public Observable<int> Hits {get; private set;} = new (0);
    public Observable<float> Damage {get; private set;} = new (0f);
    public Observable<int> Kills {get; private set;} = new (0);
    public Observable<int>[] LocalHits {get; private set;} = Enumerable.Range(0, Enum.GetValues<GC_BodyPart>().Length)
              .Select(_ => new Observable<int>())
              .ToArray();

    private void UpdateShots(int value) {Shots.Value = Fires.Sum(fire => fire.Shots);}
    private void UpdateHits(int value) {Hits.Value = Fires.Sum(fire => fire.Hits);}
    private void UpdateDamage(float value) {Damage.Value = Fires.Sum(fire => fire.Damage);}
    private void UpdateKills(int value) {Kills.Value = Fires.Sum(fire => fire.Kills);}
    private void UpdateLocalHit(int bodyPartId) =>
        LocalHits[bodyPartId].Value = Fires.Sum(fire =>
            fire.LocalHits[bodyPartId]);

    public STAT_Weapon(PW_Weapon weapon)
    {
        WeaponData = weapon.Data;
        Fires = weapon.GetFireModes()
            .Select(fire => {
                STAT_Fire stat = new(fire, weapon.Icon, weapon.IconColor);
                stat.Shots.Subscribe(UpdateShots);
                stat.Hits.Subscribe(UpdateHits);
                stat.Damage.Subscribe(UpdateDamage);
                stat.Kills.Subscribe(UpdateKills);

                foreach (var pair in stat.LocalHits.Select((value, index) => (value, index)))
                    pair.value.Subscribe(_ => UpdateLocalHit(pair.index));
                    
                return stat;
            })
            .ToArray();
    }

    public void Disable()
    {
        foreach (STAT_Fire fire in Fires)
            fire.Disable();
    }

    public void Dispose()
    {
        Disable();
    }

    public void Reset()
    {
        foreach (STAT_Fire fire in Fires)
            fire.Reset();
    }
}