using Godot;

[GlobalClass]
public partial class VFX_LineTrail : VFX_HitscanTrail
{
	[Export] private float _fadeTime = 0.5f;
	[Export] private VFX_LineType _lineType;

	public override VFX_TrailMesh CreateTrail(Material material) =>
		new VFX_LineTrailMesh(material, _fadeTime, _lineType);
}
