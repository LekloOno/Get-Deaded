using Godot;

public abstract partial class PA_Fire : PA_LayeredSound
{
    public abstract PW_Fire Fire {get;}

    public override void _Ready()
    {
        Fire.Shot += ShotSound;
    }

    /// <summary>
    /// Play the initial shot sound. Could be overwritten to define special effects, for example, playing different sounds depending on the amount of shots received.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="shots"></param>
    public virtual void ShotSound(object sender, int shots) =>
        PlayLayers();
}