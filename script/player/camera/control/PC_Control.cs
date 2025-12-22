using Godot;
using System;

[GlobalClass]
public partial class PC_Control : Node3D
{
    [ExportCategory("Settings")]
    [Export(PropertyHint.Range, "0,100,0.1")]
    private float _cmPer360 = 32f;
    [Export(PropertyHint.Range, "0,64000,1")]
    private uint _dpi = 1600;
    
    [ExportCategory("Setup")]
    [Export] private Node3D _flatDir;
    private Vector3 _eulerAngles;

    private float _realSens;

    public EventHandler<Vector2> MouseMove;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        Rotation = _eulerAngles = Vector3.Zero;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            //_flatDir.RotateY(-mouseMotion.Relative.X * _realSens);

            //_eulerAngles += new Vector3(-mouseMotion.Relative.Y * _realSens, -mouseMotion.Relative.X * _realSens, 0f);
            //_eulerAngles.X = Mathf.Clamp(_eulerAngles.X, Mathf.DegToRad(-90), Mathf.DegToRad(90));
            //Rotation = _eulerAngles;
            Vector2 cmMotion = (-mouseMotion.ScreenRelative / _dpi) * 2.54f;
            Vector2 sensMotion = cmMotion * 2 * Mathf.Pi / _cmPer360;
            RotateFlatDir(sensMotion.X);
            RotateXClamped(sensMotion.Y);
            MouseMove.Invoke(this, sensMotion);
            Transform = Transform.Orthonormalized();
        }
    }

    // Might be clamping the value an unecessary amount of time ... to improve.
    public void RotateXClamped(float theta)
    {
        RotateX(theta);
        Vector3 rotation = Rotation;
        rotation.X = Mathf.Clamp(rotation.X, Mathf.DegToRad(-90), Mathf.DegToRad(90));
        Rotation = rotation;
    }

    public void RotateFlatDir(float theta) => _flatDir.RotateY(theta);
    public Vector2 CurrentRotation() => new(_flatDir.Rotation.Y, Rotation.X);

    // Classic method .. if the manual euler method ever breaks

    //CameraY.RotateY(-mouseMotion.Relative.X * _realSens);
    //RotateX(-mouseMotion.Relative.Y * _realSens);
    //Vector3 rotation = Rotation;
    //rotation.X = Mathf.Clamp(rotation.X, Mathf.DegToRad(-90), Mathf.DegToRad(90));
    //Rotation = rotation;
}
