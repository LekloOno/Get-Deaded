using System;

public interface PHX_ListenPoolObject<T> : PHX_PoolObject where T: PHX_ListenPoolObject<T>
{
    event Action<T> Pooled;
}