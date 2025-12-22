using System;
using Godot;

namespace Pew;

public class STAT_Fire: IDisposable
{
    public Observable<int> Shots {get; private set;} = new(0);
    public Observable<int> Hits {get; private set;} = new(0);
    public Observable<int[]> LocalHits {get; private set;} = new (new int[System.Enum.GetValues<GC_BodyPart>().Length]);

    public Observable<int> Kills {get; private set;} = new(0);
    public Observable<float> Damage {get; private set;} = new(0);
    public Texture2D Icon {get;}
    public Color IconColor {get;}

    private PW_Fire _fire;

    public STAT_Fire(PW_Fire fire, Texture2D defaultIcon, Color color)
    {
        _fire = fire;
        if (fire.Icon != null)
            Icon = fire.Icon;
        else
            Icon = defaultIcon;

        IconColor = color;
        
        fire.Hit += HandleHit;
        fire.Shot += HandleShot;
    }  

    public void HandleHit(object sender, HitEventArgs hit)
    {
        if (hit.Missed)
            return;

        if (hit.IsEnv)
        {
            Shots.Value --;
            return;
        }

        Hits.Value ++;
        LocalHits.Value[(int)hit.HurtBox.BodyPart] ++;
        Damage.Value += hit.Damage;
        
        if (hit.Killed)
            Kills.Value ++;
    }

    public void HandleShot(object sender, int shots) => Shots.Value += shots;

    public void Disable()
    {
        _fire.Hit -= HandleHit;
        _fire.Shot -= HandleShot;
    }

    public void Dispose()
    {
        Disable();
    }
}