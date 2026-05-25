using Godot;

[GlobalClass]
public partial class PW_WeaponReloaderData : Resource
{
    /// <summary>
    /// The time (seconds) required to unload the weapon.
    /// 0 means the weapon do not require unloading.
    /// </summary>
    [Export] public float UnloadTime { get; private set; }
    /// <summary>
    /// The time (seconds) required to move some ammos (a magazine or individual rounds depending on the PW_Ammunition)
    /// into the weapon.
    /// </summary>
    [Export] public float InsertTime {get; private set;}
    /// <summary>
    /// The time (seconds) required to load a round in chamber.
    /// 0 means the weapons do not require manual chamber.
    /// </summary>
    [Export] public float ChamberTime { get; private set; }
    /// <summary>
    /// The time (seconds) before the weapon can be fired again once the reload is completed.
    /// </summary>
    [Export] public float RecoverTime {get; private set;}
}