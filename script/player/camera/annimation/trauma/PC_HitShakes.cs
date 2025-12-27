using Godot;

[GlobalClass]
public partial class PC_HitShakes : Node
{
    [Export] private PC_Shakeable _shakeable;
    [Export] private GE_ActiveCombatEntity _entity;

    public override void _Ready()
    {
        _entity.WeaponsHandler.Hit += HandleHit;
    }

    private void HandleHit(object sender, HitEventArgs e)
    {
        if (e.Target is not GE_ICombatEntity target)    // null check
            return;

        if (e.Killed)
            target.KillTraumaData.AddTrauma(_shakeable);
    }
}