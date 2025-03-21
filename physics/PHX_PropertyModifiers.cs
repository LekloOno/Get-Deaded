using System.Collections.Generic;

public abstract class PHX_PropertyModifier<T>
{
    protected List<T> _modifiers = [];
    public abstract float Result();
    public PHX_PropertyModifier<T> Add(T modifier)
    {
        _modifiers.Add(modifier);
        return this;
    }
    public PHX_PropertyModifier<T> Remove(T modifier)
    {
        _modifiers.Remove(modifier);
        return this;
    }

    public static PHX_PropertyModifier<T> operator +(PHX_PropertyModifier<T> wrapper, T modifier) => wrapper.Add(modifier);
    public static PHX_PropertyModifier<T> operator -(PHX_PropertyModifier<T> wrapper, T modifier) => wrapper.Remove(modifier);
}