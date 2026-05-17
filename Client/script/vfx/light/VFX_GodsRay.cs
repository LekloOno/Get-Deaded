using Godot;

[GlobalClass, Tool]
public partial class VFX_GodsRay: MeshInstance3D
{
    private uint _faces = 4;
    [Export(PropertyHint.Range, "3,16,or_greater")] public uint Faces
    {
        get => _faces;
        set
        {
            _faces = value;
            CallDeferred(MethodName.SetMesh);
        }
    }

    private float _baseRadius = 1;
    [Export] public float BaseRadius
    {
        get => _baseRadius;
        set {
            _baseRadius = value;
            CallDeferred(MethodName.SetMesh);
        }
    }

    private float _length = 1;
    [Export] public float Length
    {
        get => _length;
        set {
            _length = value;
            CallDeferred(MethodName.SetMesh);
        }
    }

    private float _initialRotation = 0;
    [Export] public float InitialRotation
    {
        get => _initialRotation;
        set {
            _initialRotation = value;
            CallDeferred(MethodName.SetMesh);
        }
    }

    private float _flareAngle = 10;
    [Export] public float FlareAngle
    {
        get => _flareAngle;
        set {
            _flareAngle = value;
            CallDeferred(MethodName.SetMesh);
        }
    }

    public ImmediateMesh GenerateFace()
    {
        ImmediateMesh drawMesh = new();
        
        drawMesh.SurfaceBegin(Mesh.PrimitiveType.Triangles);

        float theta = 2*Mathf.Pi/_faces;
        float cosTh = Mathf.Cos(theta);
        float sinTh = Mathf.Sin(theta);

        float flareAngleRad = Mathf.DegToRad(_flareAngle);
        float bottomRadiusMultiplier = (_baseRadius + _length * Mathf.Atan(flareAngleRad))/_baseRadius;

        Vector3 edgeX = new(_baseRadius, 0, 0);
        Vector3 lengthVec = new(0, _length, 0);

        if (_initialRotation != 0)
        {
            float radRot = Mathf.DegToRad(_initialRotation);
            float cosInit = Mathf.Cos(radRot);
            float sinInit = Mathf.Sin(radRot);

            edgeX = new(
                edgeX.X * cosInit - edgeX.Z * sinInit,
                0,
                edgeX.X * sinInit + edgeX.Z * cosInit
            );
        }

        for (uint i = 0; i < _faces; i ++)
        {
            Vector3 p1 = edgeX;
            Vector3 p2 = new(
                edgeX.X * cosTh - edgeX.Z * sinTh,
                0,
                edgeX.X * sinTh + edgeX.Z * cosTh
            );

            Vector3 p3 = p1 * bottomRadiusMultiplier - lengthVec;
            Vector3 p4 = p2 * bottomRadiusMultiplier - lengthVec;

            float u1 = (float)i / _faces;
            float u2 = (float)(i + 1) / _faces;

            Vector2 uv1 = new(u1, 0);
            Vector2 uv2 = new(u2, 0);
            Vector2 uv3 = new(u1, 1);
            Vector2 uv4 = new(u2, 1);
            
            drawMesh.SurfaceSetUV(uv1);
            drawMesh.SurfaceAddVertex(p1);

            drawMesh.SurfaceSetUV(uv2);
            drawMesh.SurfaceAddVertex(p2);

            drawMesh.SurfaceSetUV(uv3);
            drawMesh.SurfaceAddVertex(p3);

            drawMesh.SurfaceSetUV(uv3);
            drawMesh.SurfaceAddVertex(p3);

            drawMesh.SurfaceSetUV(uv2);
            drawMesh.SurfaceAddVertex(p2);

            drawMesh.SurfaceSetUV(uv4);
            drawMesh.SurfaceAddVertex(p4);

            edgeX = p2;
        }

        drawMesh.SurfaceEnd();
        return drawMesh;
    }

    public void SetMesh()
    {
        Mesh = GenerateFace();
    }
}