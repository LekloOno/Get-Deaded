using System.Globalization;
using System.Linq;
using Godot;

public partial class CONF_UserSettingsLoader : Node
{
    public static Variant GetSoundSetting(string key) =>
		Instance.Config.GetValue("sound", key);

	public static void RegisterSoundSetting(string key, Variant value) =>
		Instance.Config.SetValue("sound", key, value);

	public static string BusVolumeDbKey(string busName) =>
		busName.ToSnakeCase() + "_volume_db";

	public static bool IsBusVolumeDb(string key, out string busName)
	{
		if (key.EndsWith("_volume_db"))
		{
			busName = key
				.Replace("_volume_db", "")
				.Split("_")
				.Join(" ");

			busName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(busName);
			return true;
		}
		
		busName = "";
		return false;
	}

	public static void RegisterBusVolumeDb(string busName, float volumeDb)
	{
		Instance.Config.SetValue(
			"sound",
			BusVolumeDbKey(busName),
			volumeDb.ToString()
		);
	}

	public static float GetBusVolumeDb(string busName)
	{
		return (float)Instance.Config.GetValue(
			"sound",
			BusVolumeDbKey(busName)
		);
	}
        
    private void LoadSoundSettings()
	{
		foreach (string key in Config.GetSectionKeys("sound"))
		{
			var value = Config.GetValue("sound", key);

			if (IsBusVolumeDb(key, out string busName))
			{
				int busIndex = AudioServer.GetBusIndex(busName);
				AudioServer.SetBusVolumeDb(busIndex, (float)value);
				continue;
			}

			// Nothing more to do at this point, we only have volume settings
		}
	}
}