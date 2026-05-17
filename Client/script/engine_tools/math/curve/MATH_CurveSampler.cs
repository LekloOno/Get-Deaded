using Godot;

public abstract partial class MATH_CurveSampler<T> : Resource
{
    public abstract T Sample(T value);
}