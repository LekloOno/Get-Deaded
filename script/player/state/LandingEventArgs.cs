using System;

namespace Pew;

public class LandingEventArgs : EventArgs
{
    public float LandingSpeed { get; }

    public LandingEventArgs(float landingSpeed)
    {
        LandingSpeed = landingSpeed;
    }
}