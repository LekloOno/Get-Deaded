using Godot;
public partial class UI_SpatialViewport : SubViewport
{
    private Vector2I _screenSize;
    public override void _Ready()
    {
        _screenSize = GetWindow().Size;
        Size = _screenSize;
    }
}
