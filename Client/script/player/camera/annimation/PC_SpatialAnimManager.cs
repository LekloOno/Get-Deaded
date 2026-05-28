using Godot;

public partial class PC_SpatialAnimManager : Node
{
	[Export] private PC_Lean _lean;
	[Export] private PC_Spring _spring;

	public void Reset()
	{
		_lean?.Reset();
		_spring?.Reset();
	}
}
