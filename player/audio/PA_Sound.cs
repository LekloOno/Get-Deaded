using Godot;
using Godot.Collections;

[GlobalClass]
public partial class PA_Sound : AudioStreamPlayer3D
{
    [Export] private Array<AudioStream> _stepSounds = new();
    [Export] private float _minPitch;
    [Export] private float _maxPitch;
    
    protected float _pitchBaseDelta;

    public void PlaySound()
    {
        Stream = _stepSounds.PickRandom();
        PitchScale = (float)GD.RandRange(_minPitch, _maxPitch) + _pitchBaseDelta;
        Play();
    }
}