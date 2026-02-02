using Godot;

public partial class VFX_MovingLightObject : VFX_TrailMesh
{
    private float _speed;
    private float _trailSpeed;
    private QuadMesh _mesh;
    private Vector2 _edgesDistance; // X is the nearest edge, Y is the furthest edge

    private float _distance;

    public VFX_MovingLightObject(Material material, float speed, float trailSpeed, float thickness, float _inclination): base(material)
    {
        _speed = speed;
        _trailSpeed = trailSpeed;

        _mesh = new QuadMesh(){Size = new Vector2(0f, thickness)};
        Mesh = _mesh;
        Rotation = new Vector3(Mathf.DegToRad(_inclination), 0f, 0f);
    }
    
    public Vector2 EdgesDistance
    {
        get => _edgesDistance;
        set
        {
            if (value.X >= _distance - 0.2f)
                Pool();
            
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

    public override void _Process(double delta)
    {
        float frontMove = (float)delta * _speed;
        float backMove = Mathf.Lerp(_edgesDistance.X, _edgesDistance.Y, (float)delta * _trailSpeed);

        EdgesDistance = new Vector2(backMove, _edgesDistance.Y + frontMove);
    }

    protected override void SpecShoot(Vector3 origin, Vector3 hit)
    {
        Position = origin;   
        _distance = origin.DistanceTo(hit);
        _edgesDistance = Vector2.Zero;
        _mesh.Size = new Vector2(0f, _mesh.Size.Y);
        LookAt(hit, Vector3.Up);
        RotateObjectLocal(Vector3.Up, Mathf.DegToRad(90f));
    }

    protected override void SpecPool()
    {
        Visible = false;
        SetProcess(false);
    }

    public override void Spawn()
    {
        Visible = true;
        SetProcess(true);
    }

}