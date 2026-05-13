using System.Globalization;
using System.Linq;
using Godot;

public partial class CONF_UserSettingsLoader : Node
{
	public const string SoundSection = "sound";
    private const string VolumeDbSuffix = "_volume_db";

    public static Variant GetSoundSetting(string key) =>
		Instance.Config.GetValue(SoundSection, key);

	public static void RegisterSoundSetting(string key, Variant value) =>
		Instance.Config.SetValue(SoundSection, key, value);

	public static string BusVolumeDbKey(string busName) =>
		busName.ToSnakeCase() + VolumeDbSuffix;

	public static bool IsBusVolumeDb(string key, out string busName)
	{
		if (key.EndsWith(VolumeDbSuffix))
		{
			busName = key
				.Replace(VolumeDbSuffix, "")
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
			SoundSection,
			BusVolumeDbKey(busName),
			volumeDb.ToString()
		);
	}

	public static float GetBusVolumeDb(string busName)
	{
		return (float)Instance.Config.GetValue(
			SoundSection,
			BusVolumeDbKey(busName)
		);
	}
        
    private void LoadSoundSettings()
	{
		foreach (string key in Config.GetSectionKeys(SoundSection))
		{
			var value = Config.GetValue(SoundSection, key);

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