using System.Collections.Generic;
using Godot;

public partial class DATA_WeaponRegistry : Node
{
    public static DATA_WeaponRegistry Instance;
    public Dictionary<string, DATA_Weapon> Registry = [];

    const string WeaponsDataFilePath = "res://assets/game/weapon";

    public override void _EnterTree()
    {
        Instance = this;

        var dir = DirAccess.Open(WeaponsDataFilePath);

        if (dir == null)
            return;

        dir.ListDirBegin();

        while (true)
        {
            string file = dir.GetNext();

            if (file == "")
                break;

            if (!file.EndsWith(".tres"))
                continue;

            var weapon = GD.Load<DATA_Weapon>(
                $"{WeaponsDataFilePath}/{file}"
            );

            if (weapon != null)
                Registry[weapon.Id] = weapon;
        }

        dir.ListDirEnd();
    }
}