using System.Collections.Generic;

public abstract class PHX_PropertyModifier<T>
{
    protected List<T> _modifiers = [];
    public abstract float Result();
    public void AddModifier(T modifier) => _modifiers.Add(modifier);
    public bool RemoveModifier(T modifier) => _modifiers.Remove(modifier);
}