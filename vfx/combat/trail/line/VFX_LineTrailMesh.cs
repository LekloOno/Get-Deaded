using Godot;

public partial class VFX_LineTrailMesh(Vector3 origin, Vector3 hit, Material material, float fadeTime) : VFX_TrailMesh(origin, hit, material)
{
    //private SceneTreeTimer _timer;

    public override void Shoot()
    {
        ImmediateMesh drawMesh = new();
        Mesh = drawMesh;
        drawMesh.SurfaceBegin(Mesh.PrimitiveType.Lines, MaterialOverride);
        drawMesh.SurfaceAddVertex(_origin);
        drawMesh.SurfaceAddVertex(_hit);
        drawMesh.SurfaceEnd();
        //_timer = GetTree().CreateTimer(fadeTime);
        //_timer.Timeout += QueueFree;
        //GD.Print("alo");
        Anim();
        //QueueFree();
    }

    public async void Anim()
    {
        Tween opacityTween = CreateTween();
        opacityTween.TweenProperty(MaterialOverride, "albedo_color:a", 0f, fadeTime);

        await ToSignal(opacityTween, "finished");
        
        QueueFree();
    }
}