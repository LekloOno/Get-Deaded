using System;
using Godot;

[GlobalClass]
public partial class UI_Crosshair : Control
{
    [Export] private Node3D _sightPosition;
    [Export] private Camera3D _camera;
    [Export] private Control image;
    [Export] float _maxRange = 20;
    [Export] float _minRange = 1.5f;
    [Export] float _smoothdistanceThreshold = 5f;
    [Export] float _smoothPositionThreshold = 100f;
    [Export] float _smoothSpeed = 10f;

    private Vector2 _imageOffset;
    private float _distance;
    private float _targetDistance;
    private Vector2 _targetPosition;

    public override void _Ready()
    {
        _imageOffset = image.Size/2;

        Vector2 viewportSize = GetViewport().GetVisibleRect().Size;
    }

    public override void _Process(double delta)
    {
        var spaceState = _camera.GetWorld3D().DirectSpaceState;

        Vector3 origin = _sightPosition.GlobalPosition;
        Vector3 direction = - _sightPosition.GlobalBasis.Z;

        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(
            origin, 
            origin + direction * _maxRange
        );

        query.CollideWithAreas = true;
        query.CollisionMask = CONF_Collision.Masks.HitScan;

        var result = spaceState.IntersectRay(query);

        if (result.Count > 0)
        {
            Vector3 hitPosition = (Vector3)result["position"];
            _targetDistance = _sightPosition.GlobalPosition.DistanceTo(hitPosition);
            _targetDistance = Mathf.Clamp(_targetDistance, _minRange, _maxRange);
        }
        else
        {
            _targetDistance = _maxRange;
        }

        _targetPosition = _camera.UnprojectPosition(origin + direction * _targetDistance) - _imageOffset;

        if (TriggerSmoothDistance(delta) && TriggerSmoothPosition(delta))
        {
            _distance = Mathf.Lerp(_distance, _targetDistance, _smoothSpeed * (float) delta);
            _targetPosition = _camera.UnprojectPosition(origin + direction * _distance) - _imageOffset;
        }
        else
            _distance = _targetDistance;

        image.Position = _targetPosition;
    }

    private bool TriggerSmoothDistance(double delta) => Math.Abs(_distance - _targetDistance) > _smoothdistanceThreshold * (float) delta;
    private bool TriggerSmoothPosition(double delta) => _targetPosition.DistanceTo(image.Position) > _smoothPositionThreshold * (float) delta;
}