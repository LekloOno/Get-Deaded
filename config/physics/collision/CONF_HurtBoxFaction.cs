namespace Pew;

public enum CONF_HurtBoxFaction
{
    Player = 0,
    Enemy = 1,
}

public static class CONF_HurtBoxFactionExt
{
    public static uint LayerMask(this CONF_HurtBoxFaction self)
    {
        return self switch
        {
            CONF_HurtBoxFaction.Player => CONF_Collision.Layers.PlayerHurtBox,
            CONF_HurtBoxFaction.Enemy => CONF_Collision.Layers.EnnemiesHurtBox,
            _ => 0,
        };
    }
}