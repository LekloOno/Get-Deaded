using Godot;

public partial class CONF_BodyModifiers : Node
{
    public static CONF_BodyModifiers Instance { get; private set; }

    public CONFD_DefaultModifiers Defaults { get; private set; }

    public override void _EnterTree()
    {
        Instance = this;
        Defaults = (CONFD_DefaultModifiers) ResourceLoader.Load("res://config/combat/body_modifiers/conf_default_body_modifiers.tres");
    }

    public static float GetDefaultModifier(GC_BodyPart bodyPart)
    {
        return bodyPart switch
        {
            GC_BodyPart.Chest => Instance.Defaults.Chest,
            GC_BodyPart.Head =>  Instance.Defaults.Head,
            _ => Instance.Defaults.Default,
        };
    }
}