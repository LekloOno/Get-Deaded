using Godot;

namespace Pew;

[GlobalClass]
public partial class VFX_SimpleLine : VFX_LineType
{
    public override ImmediateMesh GenerateMesh(Vector3 origin, Vector3 hit, Camera3D camera)
    {
        ImmediateMesh drawMesh = new();
        
        drawMesh.SurfaceBegin(Mesh.PrimitiveType.Lines);
        drawMesh.SurfaceAddVertex(origin);
        drawMesh.SurfaceAddVertex(hit);
        
        return drawMesh;
    }
}