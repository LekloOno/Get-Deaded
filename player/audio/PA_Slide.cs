using System;
using System.Linq.Expressions;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class PA_Slide : Node3D
{
    [ExportCategory("Settings")]
    [Export] private Array<PA_Sound> _slideInLayers;
    
    [ExportCategory("Setup")]
    [Export] private PM_Slide _slide;

    public override void _Ready()
    {
        _slide.OnStart += PlaySlideIn;
    }

    public void PlaySlideIn(object sender, EventArgs e)
    {
        foreach (PA_Sound layer in _slideInLayers)
            layer.PlaySound();
    }
}