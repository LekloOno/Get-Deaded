using Godot;

[GlobalClass]
public partial class UI_PlayerHealth : UI_EntityHealth
{
    [Export] private GC_Shield _shield;
    public override void _Ready()
    {
        base._Ready();
    }
    public override void InitState(HealthInitEventArgs initState)
    {
        base.InitState(initState);
        if (initState.ReInit)
            return;

        _shield.OnDecay += Damage;
    }
}