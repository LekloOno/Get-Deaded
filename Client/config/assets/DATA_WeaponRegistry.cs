using System.Collections.Generic;
using Godot;

public partial class DATA_WeaponRegistry : Node
{
    public static DATA_WeaponRegistry Instance;
    public Dictionary<string, DATA_Weapon> Registry = [];

    const string WeaponsDataJsonRegistry = "res://config/assets/registry.json";

    public override void _EnterTree()
    {
        Instance = this;

        if (!FileAccess.FileExists(WeaponsDataJsonRegistry))
        {
            GD.PushError($"Weapon registry not found: {WeaponsDataJsonRegistry}");
            return;
        }

        using var file = FileAccess.Open(WeaponsDataJsonRegistry, FileAccess.ModeFlags.Read);
        var jsonText = file.GetAsText();

        var parsed = Json.ParseString(jsonText);

        if (parsed.VariantType != Variant.Type.Dictionary)
        {
            GD.PushError("Invalid JSON format: expected dictionary root.");
            return;
        }

        var root = parsed.AsGodotDictionary();

        if (!root.ContainsKey("weapons"))
        {
            GD.PushError("Invalid JSON: missing 'weapons' key.");
            return;
        }

        var weaponsArray = root["weapons"].AsGodotArray();

        foreach (var entry in weaponsArray)
        {
            string path = entry.AsString();

            if (string.IsNullOrEmpty(path))
                continue;

            var weapon = GD.Load<DATA_Weapon>(path);

            if (weapon == null)
            {
                GD.PushWarning($"Failed to load weapon: {path}");
                continue;
            }

            if (!string.IsNullOrEmpty(weapon.Id))
                Registry[weapon.Id] = weapon;
        }
    }
}