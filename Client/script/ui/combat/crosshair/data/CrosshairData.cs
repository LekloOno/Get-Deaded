using Godot;
using Godot.Collections;

[GlobalClass]
public partial class CrosshairData : Resource
{
    [Export] public Array<CrosshairShapeData> Shapes { get; private set; } = [];
    [Export] public bool  CombineShapes   { get; set; } = true;
    [Export] public Color FillColor       { get; set; } = Colors.White;
    [Export] public bool  CombineOutlines { get; set; } = true;
    [Export] public OutlineData OutlineData { get; private set; } = new();
}