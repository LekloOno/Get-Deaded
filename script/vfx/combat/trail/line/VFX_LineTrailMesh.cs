using Godot;

public partial class VFX_LineTrailMesh : VFX_TrailMesh
{
    private float _fadeTime;
    private VFX_LineType _lineType;
    private float _initialOpacity;

    public VFX_LineTrailMesh(
        Material material,
        float fadeTime,
        VFX_LineType lineType
    ) : base(material)
    {
        _fadeTime = fadeTime;
        _lineType = lineType;

        if (material is StandardMaterial3D std)
            _initialOpacity = std.AlbedoColor.A;
        else
            _initialOpacity = 1f;
    }

    protected override void SpecShoot(Vector3 origin, Vector3 hit)
    {
        ImmediateMesh drawMesh = _lineType.GenerateMesh(origin, hit, GetViewport().GetCamera3D());
        Mesh = drawMesh;
        drawMesh.SurfaceEnd();
        Anim();
    }

    public async void Anim()
    {
        Tween opacityTween = CreateTween();
        opacityTween.TweenProperty(MaterialOverride, "albedo_color:a", 0f, _fadeTime);

        await ToSignal(opacityTween, "finished");
        
        Pool();
    }

    protected override void SpecPool()
    {
        Visible = false;
    }

    public override void Spawn()
    {
        Visible = true;
        if (MaterialOverride is StandardMaterial3D std)
        {
            Color color = std.AlbedoColor;
            color.A = _initialOpacity;
            std.AlbedoColor = color;
        }
    }

}