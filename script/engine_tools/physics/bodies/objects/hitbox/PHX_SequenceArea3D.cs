using Godot;
using static Godot.Area3D;

[GlobalClass]
public abstract partial class PHX_SequenceArea3D : Node, PHX_ISequenceArea3D
{
    [Export] protected Area3D _area3D;
    public uint CollisionMask
    {
        get => _area3D.CollisionMask;
        set => _area3D.CollisionMask = value;
    }

    
    public uint CollisionLayer
    {
        get => _area3D.CollisionLayer;
        set => _area3D.CollisionLayer = value;
    }

    public event AreaEnteredEventHandler AreaEntered
    {
        add => _area3D.AreaEntered += value;
        remove => _area3D.AreaEntered -= value;
    }

    public event AreaExitedEventHandler AreaExited
    {
        add => _area3D.AreaExited += value;
        remove => _area3D.AreaExited -= value;
    }

    public event BodyEnteredEventHandler BodyEntered
    {
        add => _area3D.BodyEntered += value;
        remove => _area3D.BodyEntered -= value;
    }

    public event BodyExitedEventHandler BodyExited
    {
        add => _area3D.BodyExited += value;
        remove => _area3D.BodyExited -= value;
    }

    public abstract void StartSequence();
    public abstract void StopSequence();
    
    protected void Enable()
    {
        _area3D.Monitorable = true;
        _area3D.Monitoring = true;
    }
    
    protected void Disable()
    {
        _area3D.Monitorable = false;
        _area3D.Monitoring = false;
    }
}