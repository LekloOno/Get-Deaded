using Godot;

[GlobalClass]
public partial class GC_HealthManager : Node
{
    [Export] public GC_Health TopHealthLayer {get; private set;}

    public bool Damage(float damage) => TopHealthLayer.TakeDamage(damage);
    
    public override void _Ready()
    {
        // To implement
    }
    public override void _PhysicsProcess(double delta)
    {
        // To implement
    }
    public override void _Process(double delta)
    {
        // To implement
    }
}