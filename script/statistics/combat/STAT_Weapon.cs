using System.Linq;

public class STAT_Weapon
{
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
    public STAT_Fire[] Fires {get; private set;}

    public int Shots => Fires.Sum(fire => fire.Shots);
    public int Hits => Fires.Sum(fire => fire.Hits);
    public float Damage => Fires.Sum(fire => fire.Damage);
    public int Kills => Fires.Sum(fire => fire.Kills);
    public LocalHitsMerger LocalHits {get;}

    public STAT_Weapon(PW_Weapon weapon)
    {
        Fires = weapon.GetFireModes()
            .Select(fire => new STAT_Fire(fire, weapon.Icon))
            .ToArray();

        LocalHits = new LocalHitsMerger(Fires);
    }
}