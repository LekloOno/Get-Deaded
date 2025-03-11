using System;
using Godot;

public abstract class GC_Health
{
    private float _maxHealth;
    private float _currentHealth;
    private GC_Health _child;
    public EventHandler<float> OnDamage;
    public EventHandler<float> OnHeal;
    public EventHandler OnBreak;
    public EventHandler OnDie;
    public EventHandler OnFull;

    public GC_Health(float maxHealth, float initialHealth, GC_Health child)
    {
        _maxHealth = maxHealth;
        _currentHealth = initialHealth;
        _child = child;
    }

    public GC_Health(float maxHealth, GC_Health child) : this(maxHealth, maxHealth, child) {}
    public GC_Health(float maxHealth) : this(maxHealth, maxHealth, null) {}

    protected abstract float ModifyDamage(float damage);

    public bool TakeDamage(float damage)
    {
        float damageTaken = ModifyDamage(damage);
        _currentHealth -= damageTaken;
        OnDamage?.Invoke(this, damageTaken);

        if (_currentHealth > 0)
            return false;

        if (Propagate(Mathf.Abs(_currentHealth)))
            return true;
        
        _currentHealth = 0;
        OnBreak?.Invoke(this, EventArgs.Empty);
        return false;
    }

    private bool Propagate(float remaingDamage)
    {
        if (_child == null)
        {
            OnDie?.Invoke(this, EventArgs.Empty);
            return true;
        }

        return _child.TakeDamage(remaingDamage);
    }

    public float Heal(float healing)
    {
        float heal = _child.Heal(healing);
        _currentHealth += heal;
        OnHeal?.Invoke(this, heal);
        
        if (_currentHealth > _maxHealth)
        {
            float remainingHeal = _currentHealth - _maxHealth;
            _currentHealth = _maxHealth;
            OnFull?.Invoke(this, EventArgs.Empty);
            
            return remainingHeal;
        }

        return 0;
    }

    public float LowerMax()
    {
        if (_child == null)
            return _maxHealth;
        return _child.LowerMax();
    }

    public float LowerCurrent()
    {
        if (_child == null)
            return _currentHealth;
        return _child.LowerCurrent();
    }

    public float HigherMax()
    {
        if (_child == null)
            return 0;
        return _maxHealth + _child.HigherMax();
    }

    public float HigherCurrent()
    {
        if (_child == null)
            return 0;
        return _currentHealth + _child.HigherCurrent();
    }
}