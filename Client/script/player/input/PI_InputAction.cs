public interface PI_InputAction
{
    /// <summary>
    /// Called by the Map Manager of this Input Action.
    /// Should handle its proper enabling, such as initializing states.
    /// </summary>
    public void EnableAction();
    
    /// <summary>
    /// Called by the Map Manager of this Input Action.
    /// Should handle its proper disabling, such as reseting states, calling stop event, etc.
    /// </summary>
    public void DisableAction();
}