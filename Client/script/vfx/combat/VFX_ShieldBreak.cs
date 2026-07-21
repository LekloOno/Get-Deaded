using System;
using Godot;

// Attach to a full-rect ColorRect (in a CanvasLayer above your HUD) whose Material
// is a ShaderMaterial using shield_crack.gdshader. Mouse Filter should be set to Ignore.
public partial class VFX_ShieldBreak : ColorRect
{
    [Export] public GC_Shield _shield = null!;
    [Export] public Color DefaultCrackColor;
    [Export] public float DefaultGrowDuration = 0.1f;
    [Export] public float DefaultFadeDuration = 1f;

    private ShaderMaterial _material = null!;
    private float _elapsed;
    private float _totalDuration;
    private bool _isPlaying;
    private readonly RandomNumberGenerator _rng = new();

    public override void _Ready()
    {
        Visible = false;
        SetProcess(false);

        if (Material is not ShaderMaterial shader)
        {
            GD.PushError($"[{nameof(VFX_ShieldBreak)} requires the proper [{nameof(ShaderMaterial)}].");
            return;
        }

        _material = shader;
        _shield.OnBreak += OnBreak;
    }

    public override void _ExitTree()
    {
        SetProcess(false);
        Visible = false;
    }

    private void OnBreak(GC_Health senderLayer, GC_Health e)
    {
        PlayCrack();        
    }

    public void PlayCrack(Color? crackColor = null, float fadeDuration = -1f, float growDuration = -1f, Vector2? impactUv = null)
    {
        Color color = crackColor ?? DefaultCrackColor;
        float grow = growDuration > 0f ? growDuration : DefaultGrowDuration;
        float fade = fadeDuration > 0f ? fadeDuration : DefaultFadeDuration;
        Vector2 impact = impactUv ?? new Vector2(0.5f, 0.5f);

        _material.SetShaderParameter("crack_color", color);
        _material.SetShaderParameter("grow_time", grow);
        _material.SetShaderParameter("fade_time", fade);
        _material.SetShaderParameter("impact_uv", impact);
        _material.SetShaderParameter("seed", _rng.Randf() * 1000f);
        _material.SetShaderParameter("effect_time", 0f);

        _elapsed = 0f;
        _totalDuration = grow + fade;
        _isPlaying = true;

        Visible = true;
        SetProcess(true);
    }

    public override void _Process(double delta)
    {
        if (!_isPlaying)
            return;

        _elapsed += (float)delta;
        _material.SetShaderParameter("effect_time", _elapsed);

        if (_elapsed >= _totalDuration)
        {
            _isPlaying = false;
            SetProcess(false);
            Visible = false;
        }
    }
}
