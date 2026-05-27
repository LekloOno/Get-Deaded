using System;
using GaudioProcessTree;
using Godot;

public abstract partial class PA_Fire : Node3D
{
    [Export] protected AUD_Sound _sound;
    [Export] protected AUD_Sound? _lowAmmosSound;
    [Export] protected AUD_Sound? _drySound;
    [Export] private float _lowAmmosStartPitch = 1f;
    [Export] private float _lowAmmosEndPitch = 0.7f;
    [Export] private float _lowAmmosStartVolume = -20f;
    [Export] private float _lowAmmosEndVolume = 0f;
    public abstract PW_Fire Fire {get;}

    const uint MagPickRatio = 20;

    public override void _Ready()
    {
        Fire.Shot += ShotSound;
        Fire.DryShot += DrySound;
    }

    private void DrySound()
    {
        _drySound?.Play();
    }

    /// <summary>
    /// Play the initial shot sound. Could be overwritten to define special effects, for example, playing different sounds depending on the amount of shots received.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="shots"></param>
    public virtual void ShotSound(object sender, int shots)
    {
        _sound.Play();

        if (_lowAmmosSound == null)
            return;

        uint magPick = Fire.Ammos.MagazinePick;
        if (magPick == 0)
            return;

        uint loaded = Fire.Ammos.LoadedAmmos;

        uint minLoaded = (magPick * 10 + MagPickRatio - 1) / MagPickRatio;
         
        if (loaded <= minLoaded)
            PlayLowAmmos(_lowAmmosSound, loaded, minLoaded);
    }

    private void PlayLowAmmos(AUD_Sound sound, uint loaded, uint minLoaded)
    {
        float ratio = (float)loaded/minLoaded;

        float pitch = Mathf.Lerp
        (
            _lowAmmosEndPitch,
            _lowAmmosStartPitch,
            ratio
        );
        
        float volumeDb = Mathf.Lerp
        (
            _lowAmmosEndVolume,
            _lowAmmosStartVolume,
            ratio
        );

        sound.RelativeVolumeDb = volumeDb;
        sound.RelativePitchScale = pitch;
        sound.Play();
    }

}