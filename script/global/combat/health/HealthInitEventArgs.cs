using System;

public class HealthInitEventArgs : EventArgs
{
    public float LowerInitHealth {get;}
    public float TotalInitHealth {get;}
    public float LowerMaxHealth {get;}
    public float TotalMaxHealth {get;}
    public bool ReInit {get;}


    public HealthInitEventArgs(float totalInit, float lowerInit, float totalMaxHealth, float lowerMaxHealth, bool reInit = false)
    {
        TotalInitHealth = totalInit;
        LowerInitHealth = lowerInit;
        TotalMaxHealth = totalMaxHealth;
        LowerMaxHealth = lowerMaxHealth;
        ReInit = reInit;
    }
}