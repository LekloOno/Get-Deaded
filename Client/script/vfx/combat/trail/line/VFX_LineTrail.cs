using Godot;

[GlobalClass]
public partial class VFX_LineTrail : VFX_HitscanTrail
{
	[Export] private float _fadeTime = 0.5f;
	[Export] private VFX_LineType _lineType;
	[Export] private Material _material;

	public override VFX_TrailMesh CreateTrail() =>
		new VFX_LineTrailMesh((Material)_material.Duplicate(), _fadeTime, _lineType);
}
