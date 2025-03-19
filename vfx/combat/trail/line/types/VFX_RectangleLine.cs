using Godot;

[GlobalClass]
public partial class VFX_RectangleLine : VFX_LineType
{
    [Export] private float _thickness = 0.01f;
    public override ImmediateMesh GenerateMesh(Vector3 origin, Vector3 hit, Camera3D camera)
    {
        ImmediateMesh drawMesh = new();
        
        drawMesh.SurfaceBegin(Mesh.PrimitiveType.Triangles);

        Vector3 direction = (hit - origin).Normalized();

        Vector3 up = camera.GlobalBasis.Z;
        Vector3 right = direction.Cross(up).Normalized() * _thickness;

        Vector3 p1 = origin + right;
        Vector3 p2 = origin - right;
        Vector3 p3 = hit + right;
        Vector3 p4 = hit - right;
        
        drawMesh.SurfaceAddVertex(p1);
        drawMesh.SurfaceAddVertex(p2);
        drawMesh.SurfaceAddVertex(p3);

        drawMesh.SurfaceAddVertex(p3);
        drawMesh.SurfaceAddVertex(p2);
        drawMesh.SurfaceAddVertex(p4);

        return drawMesh;
    }
}