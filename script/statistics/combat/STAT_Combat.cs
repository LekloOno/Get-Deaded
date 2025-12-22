using System;
using System.Collections.Generic;
using System.Linq;

namespace Pew;

public partial class STAT_Combat: IDisposable
{
    public List<STAT_Weapon> Weapons {get; private set;}
    public STAT_Weapon MeleeWeapon {get; private set;}
    public Observable<float> Damage {get; private set;} = new(0);
    public Observable<int> Kills {get; private set;} = new(0);
    public Observable<int> Deaths {get; private set;} = new(0);

    public Action GotInitialized;

    public STAT_Combat(GE_IActiveCombatEntity entity) :
    this(entity.WeaponsHandler.Weapons, entity.WeaponsHandler.Melee, entity.HealthManager){}

    public STAT_Combat(Godot.Collections.Array<PW_Weapon> weapons, PW_Weapon melee, GC_HealthManager healthManager)
    {
        Weapons = [.. weapons
            .Select(weapon => 
            {
                STAT_Weapon stat = new(weapon);
                stat.Damage.Subscribe(UpdateDamage);
                stat.Kills.Subscribe(UpdateKills);
                return stat;
            })];
        
        MeleeWeapon = new(melee);
        MeleeWeapon.Damage.Subscribe(UpdateDamage);
        MeleeWeapon.Kills.Subscribe(UpdateKills);
        
        healthManager.TopHealthLayer.OnDie += CountDeath;
        
        GotInitialized?.Invoke();
    }
    
    private void UpdateDamage(float value) {Damage.Value = Weapons.Sum(weapon => weapon.Damage) + MeleeWeapon.Damage;}
    private void UpdateKills(int value) {Kills.Value = Weapons.Sum(weapon => weapon.Kills) + MeleeWeapon.Kills;}


    private void CountDeath(GC_Health senderLayer) => Deaths.Value ++;
    public void Disable()
    {
        MeleeWeapon.Disable();
        foreach (STAT_Weapon weapon in Weapons)
            weapon.Disable();
    }

    public void Dispose()
    {
        Disable();
    }
}