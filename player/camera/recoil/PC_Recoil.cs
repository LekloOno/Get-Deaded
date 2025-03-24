using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class PC_Recoil : Node3D
{
    [Export] public PC_Control CameraControl {get; private set;}
    [Export] private float _resetSpeed;
    public Vector2 _initialRotation;
    public Vector2 _bufferedRecoil;

    private List<PC_BaseHandler> _recoilHandlers = [];
    public override void _Process(double delta)
    {
        Vector2 appliedVel = Vector2.Zero;

        for (int i = _recoilHandlers.Count - 1; i >= 0; i--)
        {
            if (_recoilHandlers[i].Tick(delta, out Vector2 velocity))
                _recoilHandlers.RemoveAt(i);

            appliedVel += velocity;
            _bufferedRecoil += velocity;
        }

        CameraControl.RotateXClamped(appliedVel.Y);
        CameraControl.RotateFlatDir(appliedVel.X);

        //float resetDiff = (CameraControl.CurrentRotation() - _initialRotation).Length();
        //if (Mathf.Abs(resetDiff) <= 0.1f)
        //    _resetHandlers = [];
    }

    /// <summary>
    /// Add an horizontal (angle.X) and vertical (angle.Y) recoil in degrees to handle in the given time in seconds.
    /// </summary>
    /// <param name="angle">The total recoil angle in degrees, where X is the horizontal recoil, and Y is the vertical recoil.</param>
    /// <param name="time">The time before this recoil angle is reached.</param>
    /// <param name="threshold">The speed threshold below which the recoil will be reset. 0f by default</param>
    /// <param name="autoRemove">The recoil will be automatically freed when it passes its threshold. If set to false, the caller of this method is responsible for freeing it.</param>
    /// <returns>The created recoil handler layer.</returns>
    public PC_RecoilHandler AddRecoil(Vector2 angle, float time, float threshold = 0f, bool autoRemove = true)
    {
        Vector2 radAngle = new(Mathf.DegToRad(angle.X), Mathf.DegToRad(angle.Y));
        PC_RecoilHandler recoilHandler = new(radAngle, time, autoRemove);
        _recoilHandlers.Add(recoilHandler);
        return recoilHandler;
    }

    public void RemoveRecoil(PC_RecoilHandler _handler) => _recoilHandlers.Remove(_handler);

    public void ResetBuffer() => _bufferedRecoil = Vector2.Zero;

    /// <summary>
    /// Resets the buffered recoil. Call ResetBuffer() accordingly.
    /// </summary>
    /// <param name="time">The time required to complete the reset.</param>
    public void ResetRecoil(float time) => _recoilHandlers.Add(new PC_ResetHandler(_bufferedRecoil, time));
}