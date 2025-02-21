using Godot;

[GlobalClass]
public partial class UI_Crosshair : Node3D
{
    [Export] private Node3D _sightPosition;
    [Export] private Camera3D _camera;
    [Export] private Control image;
    [Export] float _maxRange = 30;
    [Export] float _minRange = 5;
    [Export] float _resetSpeed = 5;

    private Vector2 _imageOffset;
    private Vector2 _initPosition;
    private Vector2 _targetPosition;

    public override void _Ready()
    {
        _imageOffset = image.Size/2;

        Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
        _initPosition = viewportSize/2 - _imageOffset;
    }

    public override void _Process(double delta)
    {
        var spaceState = GetWorld3D().DirectSpaceState;

        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(
            _sightPosition.GlobalPosition, 
            _sightPosition.GlobalPosition - _sightPosition.GlobalBasis.Z * _maxRange
        );

        var result = spaceState.IntersectRay(query);

        if (result.Count > 0)
        {
            Vector3 hitPosition = (Vector3)result["position"];
            float distance = _sightPosition.GlobalPosition.DistanceTo(hitPosition);

            if(_camera.IsPositionBehind(hitPosition)) GD.Print("oui");

            if (distance > _minRange)
            {
                _targetPosition = _camera.UnprojectPosition(hitPosition)  - _imageOffset;
            }
            else
            {
                Vector3 minRangePosition = _sightPosition.GlobalPosition - _sightPosition.GlobalBasis.Z * _minRange;
                _targetPosition = _camera.UnprojectPosition(minRangePosition) - _imageOffset;
                //image.Position = Vector2.Zero;
            }
        }
        else
        {
            _targetPosition = _initPosition;
        }

        image.Position = image.Position.Lerp(_targetPosition, (float)delta * _resetSpeed);
    }
}