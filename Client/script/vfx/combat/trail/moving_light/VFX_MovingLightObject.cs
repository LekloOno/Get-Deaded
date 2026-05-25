using Godot;

public partial class VFX_MovingLightObject : VFX_TrailMesh
{
    private float _speed;
    private float _trailSpeed;
    private Vector2 _edgesDistance; // X is the nearest edge, Y is the furthest edge

    private float _distance;

    public VFX_MovingLightObject(ShaderMaterial material, float speed, float trailSpeed, float thickness, float _inclination): base(material)
    {
        _speed = speed;
        _trailSpeed = trailSpeed;
        Mesh = new QuadMesh(){Size = new Vector2(1f, thickness)};
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

            SetInstanceShaderParameter("near_edge", value.X);
            SetInstanceShaderParameter("far_edge", cappedY);

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
        EdgesDistance = Vector2.Zero;
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