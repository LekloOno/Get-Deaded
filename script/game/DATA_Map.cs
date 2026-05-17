using Godot;

[GlobalClass]
public partial class DATA_Map : Resource
{
    public DATA_Map() {}
    public DATA_Map(string id, string displayName)
    {
        Id = id;
        DisplayName = displayName;
    }

    [Export] public string Id {get; private set;}
    [Export] public string DisplayName {get; private set;}
}