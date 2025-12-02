using Godot;

public partial class CONF_HitColors : Node
{
    public static CONF_HitColors Instance { get; private set; }

    public CONFD_HitColors HitColors { get; private set; }

    public static CONFD_HitColors Colors { get => Instance.HitColors; }

    public override void _EnterTree()
    {
        Instance = this;
        HitColors = (CONFD_HitColors) ResourceLoader.Load("res://config/combat/hit/conf_hit_colors.tres");
    }
}