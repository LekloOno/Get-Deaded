[System.Flags]
public enum CONFE_HitMasks
{
    Enemies = 1 << 0,
    Player = 1 << 1,
}

public static class CONFE_HitMasksExtensions
{
    public static uint ToCollisionMask(this CONFE_HitMasks hitMask)
    {
        uint result = 0;

        if (hitMask.HasFlag(CONFE_HitMasks.Enemies))
            result |= CONF_Collision.Layers.EnnemiesHurtBox;

        if (hitMask.HasFlag(CONFE_HitMasks.Player))
            result |= CONF_Collision.Layers.PlayerHurtBox;

        return result;
    }
}