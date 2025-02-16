using Godot;
using System;

public partial class PC_Control : Camera3D
{
	[Export] public float Sensitivity {get;set;} = 2.8f;
	[Export] public Node3D Body {get; private set;}

	private float _realSens;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_realSens = Sensitivity / 6500f;
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion && Body != null)
        {
            Body.RotateY(-mouseMotion.Relative.X * _realSens);
            RotateX(-mouseMotion.Relative.Y * _realSens);
            
            Vector3 rotation = Rotation;
            rotation.X = Mathf.Clamp(rotation.X, Mathf.DegToRad(-90), Mathf.DegToRad(90));
            Rotation = rotation;
        }
    }
}
