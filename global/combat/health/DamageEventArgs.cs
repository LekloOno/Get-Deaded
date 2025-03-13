using System;

public class DamageEventArgs : EventArgs
{
    public float Amount {get;}
    public float CurrentHealth {get;}

    public DamageEventArgs(float amount, float currentHealth)
    {
        Amount = amount;
        CurrentHealth = currentHealth;
    }
}