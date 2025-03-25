using Godot;
using Godot.Collections;

[GlobalClass]
public partial class GL_DropTable : Resource
{
    [Export] public Array<GL_DropItem> Table {get; private set;}
}