using Godot;
using System;
using System.Runtime;

[GlobalClass]
public partial class PC_Control : Node3D
{
    [ExportCategory("Settings")]
    [Export(PropertyHint.Range, "0,100,0.05")]
    private float _sensitivity = 2.8f;
    
    [ExportCategory("Setup")]
    [Export] private Node3D _flatDir;
    private Vector3 _eulerAngles;

    private float _realSens;

    public override void _Ready()
    {
        _realSens = _sensitivity / 6500f;
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
            RotateFlatDir(-mouseMotion.Relative.X * _realSens);
            RotateXClamped(-mouseMotion.Relative.Y * _realSens);
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

    // Classic method .. if the manual euler method ever breaks

    //CameraY.RotateY(-mouseMotion.Relative.X * _realSens);
    //RotateX(-mouseMotion.Relative.Y * _realSens);
    //Vector3 rotation = Rotation;
    //rotation.X = Mathf.Clamp(rotation.X, Mathf.DegToRad(-90), Mathf.DegToRad(90));
    //Rotation = rotation;
}
