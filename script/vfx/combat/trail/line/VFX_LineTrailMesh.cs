using Godot;

public partial class VFX_LineTrailMesh(Vector3 origin, Vector3 hit, Material material, float fadeTime, VFX_LineType _lineType) : VFX_TrailMesh(origin, hit, material)
{
    public override void Shoot()
    {
        ImmediateMesh drawMesh = _lineType.GenerateMesh(_origin, _hit, GetViewport().GetCamera3D());
        Mesh = drawMesh;
        drawMesh.SurfaceEnd();
        Anim();
    }

    public async void Anim()
    {
        Tween opacityTween = CreateTween();
        opacityTween.TweenProperty(MaterialOverride, "albedo_color:a", 0f, fadeTime);

        await ToSignal(opacityTween, "finished");
        
        QueueFree();
    }
}