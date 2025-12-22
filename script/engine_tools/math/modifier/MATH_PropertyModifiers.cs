using System.Collections.Generic;

namespace Pew;

public abstract class MATH_PropertyModifier<T>
{
    protected List<T> _modifiers = [];
    public abstract T Result();
    public MATH_PropertyModifier<T> Add(T modifier)
    {
        _modifiers.Add(modifier);
        return this;
    }
    public MATH_PropertyModifier<T> Remove(T modifier)
    {
        _modifiers.Remove(modifier);
        return this;
    }

    public static MATH_PropertyModifier<T> operator +(MATH_PropertyModifier<T> wrapper, T modifier) => wrapper.Add(modifier);
    public static MATH_PropertyModifier<T> operator -(MATH_PropertyModifier<T> wrapper, T modifier) => wrapper.Remove(modifier);
}