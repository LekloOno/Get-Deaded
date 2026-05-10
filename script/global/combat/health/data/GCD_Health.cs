using Godot;

[GlobalClass]
public partial class GCD_Health : Resource
{
    [Export] public float MaxHealth {get; private set;}
    [Export] public GCD_Health Child {get; private set;}

    public virtual GC_Health BuildNode() =>
        new GC_Health(MaxHealth, Child?.BuildNode());
}