using System.Linq;
using Godot;
using Godot.Collections;

namespace Pew;

[GlobalClass]
public partial class PA_Sound : AudioStreamPlayer3D
{
    [Export] private Array<AudioStream> _stepSounds = new();
    [Export] private float _minPitch = 1f;
    [Export] private float _maxPitch = 1f;
    
    protected float _pitchBaseDelta;

    public void PlaySound()
    {
        Stream = _stepSounds.PickRandom();
        PitchScale = (float)GD.RandRange(_minPitch, _maxPitch) + _pitchBaseDelta;
        Play();
    }
}