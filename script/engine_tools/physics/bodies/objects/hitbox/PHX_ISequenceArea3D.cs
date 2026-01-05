using static Godot.Area3D;

/// <summary>
/// Defines a sequencing of Area3D, typically used to animate some hitboxes. <br/>
/// For example, it could be a "hammer swing" sequence, that moves and enables/disables different hitboxes through time.
/// </summary>
public interface PHX_ISequenceArea3D
{
    /// <summary>
    /// The physics layers this PHX_ISequenceArea3D scans.
    /// </summary>
    uint CollisionMask {get; set;}
    /// <summary>
    /// The physics layers this PHX_ISequenceArea3D is in.
    /// </summary>
    uint CollisionLayer {get; set;}
    /// <summary>
    /// Emitted when the received <c>area</c> starts interracting with this sequencer
    /// </summary>
    event AreaEnteredEventHandler AreaEntered;
    /// <summary>
    /// Emitted when the received <c>area</c> stops interracting with this sequencer.
    /// </summary>
    event AreaExitedEventHandler AreaExited;
    /// <summary>
    /// Emitted when the received <c>body</c> starts interracting with this sequencer. <br/>
    /// <c>body</c> can be a PhysicsBody3D or a GridMap. <br/>
    /// GridMaps are detected if their MeshLibrary has collision shapes configured. <br/>
    /// </summary>
    event BodyEnteredEventHandler BodyEntered;
    /// <summary>
    /// Emitted when the received <c>body</c> stops interracting with this sequencer. <br/>
    /// <c>body</c> can be a PhysicsBody3D or a GridMap. <br/>
    /// GridMaps are detected if their MeshLibrary has collision shapes configured. <br/>
    /// </summary>
    event BodyExitedEventHandler BodyExited;
    /// <summary>
    /// Starts the Area3D sequence. <br/>
    /// <br/>
    /// Will eventually cause the firing of Area/Body Enter/Exit events.
    /// </summary>
    void StartSequence();
    /// <summary>
    /// Stops the Area3D sequence. <br/>
    /// <br/>
    /// The procedure might require some time, and it is not guaranteed that no Area/Body Enter/Exit events will fire anymore instantly. <br/>
    /// It is up to the implementor to define precisely what stopping the sequence does.
    /// </summary>
    void StopSequence();
}