using Godot;

public partial class VFX_MovingLightObject : Node3D
{
    private float _speed;
    private float _trailSpeed;
    private QuadMesh _mesh;
    private MeshInstance3D _meshInstance;
    private Vector2 _edgesDistance; // X is the nearest edge, Y is the furthest edge
    private Vector3 _origin;
    private Vector3 _hit;

    private float _distance;

    public VFX_MovingLightObject(Vector3 origin, Vector3 hit, Material material, float speed, float trailSpeed, float thickness, float _inclination)
    {
        TopLevel = true;

        _origin = origin;
        _hit = hit;

        _speed = speed;
        _trailSpeed = trailSpeed;
        _distance = origin.DistanceTo(hit);
        _edgesDistance = Vector2.Zero;

        _mesh = new QuadMesh(){Size = new Vector2(0f, thickness)};

        _meshInstance = new(){
            Mesh = _mesh,
            MaterialOverride = material,
            Rotation = new Vector3(Mathf.DegToRad(_inclination), Mathf.DegToRad(90f), 0f),
        };
    }
    
    public Vector2 EdgesDistance
    {
        get => _edgesDistance;
        set
        {
            if (value.X >= _distance - 0.2f)
                QueueFree();
            
            float cappedY = Mathf.Min(_distance, value.Y);

            Vector2 size = _mesh.Size;
            size.X = cappedY - value.X;

            Vector3 offset = _mesh.CenterOffset;
            offset.X = (size.X/2f) + value.X;

            _mesh.Size = size;
            _mesh.CenterOffset = offset;

            _edgesDistance = value;
        }
    }

    public override void _Ready()
    {
        AddChild(_meshInstance);
        Position = _origin;
        LookAt(_hit, Vector3.Up);
    }

    public override void _Process(double delta)
    {
        float frontMove = (float)delta * _speed;
        float backMove = Mathf.Lerp(_edgesDistance.X, _edgesDistance.Y, (float)delta * _trailSpeed);

        EdgesDistance = new Vector2(backMove, _edgesDistance.Y + frontMove);
    }
}