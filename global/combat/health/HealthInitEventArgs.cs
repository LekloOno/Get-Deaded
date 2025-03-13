using System;

public class HealthInitEventArgs : EventArgs
{
    public float LowerInitHealth {get;}
    public float TotalInitHealth {get;}
    public float LowerMaxHealth {get;}
    public float TotalMaxHealth {get;}


    public HealthInitEventArgs(float totalInit, float lowerInit, float totalMaxHealth, float lowerMaxHealth)
    {
        TotalInitHealth = totalInit;
        LowerInitHealth = lowerInit;
        TotalMaxHealth = totalMaxHealth;
        LowerMaxHealth = lowerMaxHealth;
    }
}