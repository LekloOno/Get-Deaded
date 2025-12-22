using Godot;

namespace Pew;

[GlobalClass]
public abstract partial class PCT_DirectCauser : Node
{
    [Export] protected PC_Shakeable _shakeableCamera;

    public void Initialize(PC_Shakeable shakeableCamera)
    {
        _shakeableCamera = shakeableCamera;
        InnerInitialize();
    }

    protected virtual void InnerInitialize(){}
}