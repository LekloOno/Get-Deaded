using Godot;
using System;

[GlobalClass]
public partial class PC_Control : Node3D
{
    [Export] private PC_Settings _settings = null!;
    
    [ExportCategory("Setup")]
    [Export] private Node3D _flatDir = null!;

    private float _rawPitch = 0f;
    private float _rawYaw   = 0f;

    private Vector2 _recoilPrevious = Vector2.Zero; // X = yaw, Y = pitch
    private Vector2 _recoilCurrent  = Vector2.Zero;

    public event Action<Vector2>? MouseMoved;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            Vector2 sensMotion = - mouseMotion.ScreenRelative * _settings.Sensitivity;
            _rawYaw += sensMotion.X;
            _rawPitch += sensMotion.Y;

            MouseMoved?.Invoke(sensMotion);
        }
    }

    public override void _Process(double delta) =>
        ApplyTransforms();

    public void InitRotation(Vector3 rotation)
    {
        ResetRecoil();
        _rawYaw = rotation.Y;
        _rawPitch = rotation.X;
    }

    public void RotateXClamped(float theta) => _rawPitch += theta;
    public void RotateFlatDir(float theta) => _rawYaw += theta;
    public Vector2 CurrentRotation() => new(_rawYaw, _rawPitch);

    public void SetRecoilState(Vector2 recoil)
    {
        _recoilPrevious = _recoilCurrent;
        _recoilCurrent = recoil;
    }

    public void AddRecoilState(Vector2 recoil) =>
        SetRecoilState(_recoilCurrent + recoil);
    
    private const uint ResetMask = 255;
    /// <summary>
    /// Recoil and pitch can drift away at high values
    /// I simply want to ensure we don't get in floating precision hell territory
    /// thus periodically recenter yaw, pitch and recoil.
    /// </summary>
    public override void _PhysicsProcess(double delta)
    {
        if ((Engine.GetPhysicsFrames() & ResetMask) == ResetMask)
            ResetRecoil();
    }

    private void ResetRecoil()
    {
        _rawPitch = Mathf.Clamp(
            _rawPitch + _recoilCurrent.Y,
            Mathf.DegToRad(-90f),
            Mathf.DegToRad(90f)
        );

        _rawYaw += _recoilCurrent.X;
        _rawYaw = WrapAngle(_rawYaw);

        _recoilPrevious -= _recoilCurrent;
        _recoilCurrent  = Vector2.Zero;
    }

    private static float WrapAngle(float angle) =>
        ((angle + Mathf.Pi) % Mathf.Tau + Mathf.Tau) % Mathf.Tau - Mathf.Pi;

    private void ApplyTransforms()
    {
        float alpha = (float)Engine.GetPhysicsInterpolationFraction();
        Vector2 recoil = _recoilPrevious.Lerp(_recoilCurrent, alpha);

        _rawPitch = Mathf.Clamp(
            _rawPitch,
            Mathf.DegToRad(-90f) - recoil.Y,
            Mathf.DegToRad(90f) - recoil.Y
        );

        _flatDir.Rotation = new Vector3(0f, _rawYaw + recoil.X, 0f);

        float finalPitch = _rawPitch + recoil.Y;

        Rotation = new Vector3(finalPitch, 0f, 0f);
    }
}
