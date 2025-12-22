using System;

namespace Pew;

public class DamageEventArgs : EventArgs
{
    public float Amount {get;}
    public float CurrentHealth {get;}
    public float LowerCurrentHealth {get;}
    public float TotalCurrentHealth {get; set;}

    public DamageEventArgs(float amount, GC_Health layer)
    {
        Amount = amount;
        CurrentHealth = layer.CurrentHealth;
        TotalCurrentHealth = layer.TotalCurrent(out float lowerCurrent);
        LowerCurrentHealth = lowerCurrent;
    }

    /// <summary>
    /// Used when propagating up a Damage/Heal event. This allows to compute the total health.
    /// </summary>
    /// <param name="layerHealth"></param>
    /// <returns></returns>
    public DamageEventArgs Stack(float layerHealth)
    {
        TotalCurrentHealth += layerHealth;
        return this;
    }
}