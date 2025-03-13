using System;

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

    public DamageEventArgs Stack(float layerHealth)
    {
        TotalCurrentHealth += layerHealth;
        return this;
    }
}