using Godot;

[GlobalClass]
public partial class GCD_Health : Resource
{
    [Export] public float MaxHealth {get; private set;}
    [Export] public GCD_Health Child {get; private set;}

    public GC_Health BuildNode()
    {
        if (Child == null)
            return BuildSelf(null);
        
        GC_Health child = Child.BuildNode();
        GC_Health node = BuildSelf(child);
        node.AddChild(child);
        return node;
    }
    
    protected virtual GC_Health BuildSelf(GC_Health child) =>
        new GC_Health(MaxHealth, child);
}