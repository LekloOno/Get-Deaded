using Godot;
using System;

public partial class PC_Control : Camera3D
{
    [Export] public float Sensitivity {get;set;} = 2.8f;
    [Export] public Node3D CameraY {get; private set;}
    [Export] public Node3D Body {get; private set;}
    private Vector3 _eulerAngles;

    private float _realSens;

    public override void _Ready()
    {
        _realSens = Sensitivity / 6500f;
        Input.MouseMode = Input.MouseModeEnum.Captured;
        Rotation = _eulerAngles = Vector3.Zero;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            Body.RotateY(-mouseMotion.Relative.X * _realSens);
            //CameraY.RotateY(-mouseMotion.Relative.X * _realSens);

            //RotateX(-mouseMotion.Relative.Y * _realSens);
            _eulerAngles += new Vector3(-mouseMotion.Relative.Y * _realSens, -mouseMotion.Relative.X * _realSens, 0f);
            _eulerAngles.X = Mathf.Clamp(_eulerAngles.X, Mathf.DegToRad(-90), Mathf.DegToRad(90));
            Rotation = _eulerAngles;

            //Vector3 rotation = Rotation;
            //rotation.X = Mathf.Clamp(rotation.X, Mathf.DegToRad(-90), Mathf.DegToRad(90));

            //Rotation = rotation;
        }
    }
}
