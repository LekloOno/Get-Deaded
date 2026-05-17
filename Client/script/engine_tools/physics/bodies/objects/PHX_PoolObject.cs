public interface PHX_PoolObject
{
    /// <summary>
    /// A procedure to put the object in pool. <br/>
    /// Typically disabling physics process, hiding, etc.
    /// </summary>
    void Pool();
    /// <summary>
    /// A procedure to bring the object out of the pool.
    /// Typically enabling physics process, showing, etc.
    /// </summary>
    void Spawn();
}