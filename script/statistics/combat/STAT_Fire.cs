using Godot;

public class STAT_Fire
{
    public Observable<int> Shots {get; private set;} = new(0);
    public Observable<int> Hits {get; private set;} = new(0);
    public Observable<int[]> LocalHits {get; private set;} = new (new int[System.Enum.GetValues<GC_BodyPart>().Length]);

    public Observable<int> Kills {get; private set;} = new(0);
    public Observable<float> Damage {get; private set;} = new(0);
    public Texture2D Icon {get;}

    public STAT_Fire(PW_Fire fire, Texture2D defaultIcon)
    {
        if (fire.Icon != null)
            Icon = fire.Icon;
        else
            Icon = defaultIcon;
    }

    public void Initialize(PW_Fire fire)
    {
        fire.Hit += HandleHit;
    }    

    public void HandleHit(object sender, ShotHitEventArgs hit)
    {
        if (hit == ShotHitEventArgs.MISS)
            return;

        if (hit == ShotHitEventArgs.ENV)
        {
            Shots.Value --;
            return;
        }

        Hits.Value ++;
        LocalHits.Value[(int)hit.HurtBox.BodyPart] ++;
        Damage.Value += hit.Damage;
        
        if (hit.Kill)
            Kills.Value ++;
    }

    public void HandleShot(object sender, int shots) => Shots.Value += shots;
}