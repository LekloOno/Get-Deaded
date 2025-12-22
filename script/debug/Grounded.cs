using Godot;
using System;

namespace Pew;

public partial class Grounded : Label
{
    [Export] private PS_Grounded _groundState;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        SetText(_groundState.IsGrounded() ? "GROUNDED" : "AIRBORNE");
    }
}
