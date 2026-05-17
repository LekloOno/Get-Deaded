public interface PM_IOmniLoader
{
    PM_OmniCharge Charge {set;}
    /// <summary>
    /// Notifies the loader that this amount of charge has just been consumed.
    /// </summary>
    /// <param name="charge">The charge consumed.</param>
    void Consumed(float charge);
    /// <summary>
    /// Notifies the loader that this amount of charge was requested to be consumed, but could not.
    /// </summary>
    /// <param name="charge">The charge requested, not consumed.</param>
    void TriedConsume(float charge);
}