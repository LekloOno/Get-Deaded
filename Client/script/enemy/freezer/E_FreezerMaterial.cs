using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class E_FreezerMaterial : E_EnemyMaterial
{
	[Export] private MeshInstance3D _surfaceMesh = null!;
	[Export] private MeshInstance3D _headMesh = null!;
    [Export] private float _hideDelay = 0.8f;
    [Export] private float _outlinesHideDelay = 0.1f;
    [Export] private float _showDelay = 0.3f;
    [Export] private float _outlinesShowDelay = 0.3f;

    private ShaderMaterial? _outlines;
    private ShaderMaterial? _xRay;

    private Tween? _surfaceFadeInTween;
    private Tween? _surfaceFadeOutTween;
    private Tween? _outlinesFadeInTween;
    private Tween? _outlinesFadeOutTween;

    private float _alpha = 1f;
    public float Alpha
	{
		get => _alpha;
		set
		{
            _alpha = value;
			_surfaceMesh.SetInstanceShaderParameter("alpha_override", value);
			_headMesh.SetInstanceShaderParameter("alpha_override", value);
		}
	}

    private float _outlinesAlpha = 1f;
    public float OutlinesAlpha
    {
        get => _outlinesAlpha;
		set
		{
            _outlinesAlpha = value;
			_surfaceMesh.SetInstanceShaderParameter("outlines_alpha_override", value);
			_headMesh.SetInstanceShaderParameter("outlines_alpha_override", value);
            _surfaceMesh.SetInstanceShaderParameter("xray_alpha_override", value);
            _headMesh.SetInstanceShaderParameter("xray_alpha_override", value);
		}
    }

    protected override void ReadySpec()
    {
        if (_surfaceMesh == null)
        {
            GD.PushError("Missing surface mesh on E_BasicEnemyMaterial.");
            return;
        }

        if (_headMesh == null)
        {
            GD.PushError("Missing joint mesh on E_BasicEnemyMaterial.");
            return;
        }
    }

    protected override void OnDamaged(E_IEnemy enemy, GC_Health senderLayer, DamageEventArgs e)
    {
        //
    }

    protected override void OnDied(E_IEnemy? enemy, GC_Health? senderLayer)
    {
        _outlinesFadeOutTween?.Kill();
        _outlinesFadeInTween?.Kill();

        _outlinesFadeOutTween = CreateTween();
        _outlinesFadeOutTween.TweenProperty(this, "OutlinesAlpha", 0f, _outlinesHideDelay);

        _surfaceFadeInTween?.Kill();
        _surfaceFadeOutTween?.Kill();

		_surfaceFadeOutTween = CreateTween();
		_surfaceFadeOutTween.TweenProperty(this, "Alpha", 0f, _hideDelay);

    }

    public override async Task SmoothDisableSpec()
    {
        if (_surfaceFadeOutTween == null || !_surfaceFadeOutTween.IsRunning())
            OnDied(null, null);

        await ToSignal(_surfaceFadeOutTween, "finished");
    }

    protected override void OnDisabled(E_IEnemy enemy)
    {
        _surfaceFadeInTween?.Kill();
        _surfaceFadeOutTween?.Kill();
        _outlinesFadeInTween?.Kill();
        _outlinesFadeOutTween?.Kill();

        Alpha = 0;
        OutlinesAlpha = 0;

        _surfaceMesh.Visible = false;
        _headMesh.Visible = false;
    }

    protected override void OnSpawned()
    {
        Alpha = 0;
        OutlinesAlpha = 0;

        _surfaceMesh.Visible = true;
        _headMesh.Visible = true;
        
        _surfaceFadeOutTween?.Kill();
        _surfaceFadeInTween?.Kill();

        _surfaceFadeInTween = CreateTween();
		_surfaceFadeInTween.TweenProperty(this, "Alpha", 1f, _showDelay);

        _outlinesFadeOutTween?.Kill();
        _outlinesFadeInTween?.Kill();

        _outlinesFadeInTween = CreateTween();
        _outlinesFadeInTween.TweenProperty(this, "OutlinesAlpha", 1f, _outlinesShowDelay);
    }

    private bool CheckSetup()
    {
        if (_surfaceMesh == null)
        {
            GD.PushError("Missing surface mesh on E_BasicEnemyMaterial.");
            return false;
        }


        if(_surfaceMesh.MaterialOverride is not Material sfMat)
        {
            GD.PushError("Missing material override on E_BasicEnemyMaterial's surface mesh.");
            return false;
        }

        if (_headMesh == null)
        {
            GD.PushError("Missing joint mesh on E_BasicEnemyMaterial.");
            return false;
        }


        if(_headMesh.MaterialOverride is not Material jtMat)
        {
            GD.PushError("Missing joint override on E_BasicEnemyMaterial's surface mesh.");
            return false;
        }

        return true;
    }
}