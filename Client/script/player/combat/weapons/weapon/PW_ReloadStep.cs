public enum PW_ReloadStep
{
    Ready,
    Unload,
    Insert,
    Chamber,
    Recover,
}

public static class PW_ReloadStepExtensions
{
    public static float ReloadTime(this PW_ReloadStep step, PW_WeaponReloaderData data)
    {
        float time = 0f;
        
        switch(step)
        {
            case PW_ReloadStep.Unload:
                time += data.UnloadTime;
                goto case PW_ReloadStep.Insert;

            case PW_ReloadStep.Insert:
                time += data.InsertTime;
                goto case PW_ReloadStep.Chamber;

            case PW_ReloadStep.Chamber:
                time += data.ChamberTime;
                goto case PW_ReloadStep.Recover;

            case PW_ReloadStep.Recover:
                time += data.RecoverTime;
                break;
        }

        return time;
    }

    public static float StepTime(this PW_ReloadStep step, PW_WeaponReloaderData data)
    {       
        return step switch
        {
            PW_ReloadStep.Unload => data.UnloadTime,
            PW_ReloadStep.Insert => data.InsertTime,
            PW_ReloadStep.Chamber => data.ChamberTime,
            PW_ReloadStep.Recover => data.RecoverTime,
            _ => 0f,
        };
    }
}