using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class PC_Recoil : Node3D
{
    [Export] public Vector2 Resistance {get; set;}
    [Export] public float StopThreshold {get; private set;}
    [Export] public PC_Control CameraControl {get; private set;}
    [Export] private float _resetSpeed;
    private List<PC_RecoilHandler> _recoilHandlers = [];
    public override void _Process(double delta)
    {
        Vector2 appliedVel = Vector2.Zero;

        for (int i = _recoilHandlers.Count - 1; i >= 0; i--)
        {
            if (_recoilHandlers[i].Tick(delta, out Vector2 velocity))
                _recoilHandlers.RemoveAt(i);

            appliedVel += velocity;
        }

        CameraControl.RotateXClamped(appliedVel.Y);
        CameraControl.RotateFlatDir(appliedVel.X);
    }

    public PC_RecoilHandler AddRecoil(Vector2 angle, float time, float threshold = .05f)
    {
        PC_RecoilHandler recoilHandler = new(angle, time, threshold);
        _recoilHandlers.Add(recoilHandler);
        return recoilHandler;
    }
}