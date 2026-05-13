using Godot;

[GlobalClass]
public partial class CONF_UserSettingsLoader : Node
{
	const string SettingsFilePath = "user://settings.ini";
	public ConfigFile Config {get; private set;}
	public static CONF_UserSettingsLoader Instance {get; private set;}
	private static PC_Settings _playerCameraSettings;

	public override void _EnterTree()
	{
		Instance = this;

		Config = new();
		// Not ideal
		_playerCameraSettings = ResourceLoader.Load<PC_Settings>("res://config/player/player_camera_settings.tres");

		if (FileAccess.FileExists(SettingsFilePath))
			Config.Load(SettingsFilePath);
	}

	// we could do some fancy stuffs with enums, but at the end of the day, we have to
	// manipulate strings from the actual config file. I don't think the boilerplate and instanciation cost is worth it.
	// The settings should probably not scale that much.
	public static Variant GetSetting(string section, string key) =>
		Instance.Config.GetValue(section, key);

	public static void RegisterSetting(string section, string key, Variant value) =>
		Instance.Config.GetValue(section, key, value);

	public static void Apply()
	{
		Instance.Config.Save(SettingsFilePath);
	}

	public static void Abort()
	{
		if (!FileAccess.FileExists(SettingsFilePath))
			return;

		Instance.Config.Load(SettingsFilePath);
		Load();
	}

	public static void Load()
	{
		Instance.LoadVideoSettings();
		Instance.LoadSoundSettings();
		Instance.LoadControlSettings();
	}
}
