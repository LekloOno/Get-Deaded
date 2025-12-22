using Godot;
using Godot.Collections;

namespace Pew;

[GlobalClass]
public partial class PA_LayeredSound : Node3D
{
    [Export] private Array<PA_Sound> _slideInLayers;
    public void PlayLayers()
    {
        foreach (PA_Sound layer in _slideInLayers)
            layer.PlaySound();
    }

    public void StopLayers()
    {
        foreach (PA_Sound layer in _slideInLayers)
            layer.Stop();
    }
}