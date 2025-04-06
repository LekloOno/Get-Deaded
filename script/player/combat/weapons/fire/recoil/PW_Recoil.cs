using Godot;

[GlobalClass]
public abstract partial class PW_Recoil : Resource
{
    public MATH_AdditiveModifiers Modifier {get; protected set;} = new();
    public abstract void Initialize(PC_Recoil _recoilController);
    public abstract void Start();
    public abstract void Add();
    public abstract void Reset();
    public abstract void ResetBuffer();
}