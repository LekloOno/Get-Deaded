using System;
using System.Collections.Generic;
using Godot;
using TraGUS;

[GlobalClass]
public partial class CONF_UserSettingsLoader : Node
{
	const string SettingsFilePath = "user://settings.ini";
	const string DefaultSettingsFilePath = "res://config/user/default_settings.ini";
	public ConfigFile Config {get; private set;}
	public ConfigFile DefaultConfig {get; private set;}
	public static CONF_UserSettingsLoader Instance {get; private set;}
	private static PC_Settings _playerCameraSettings;

	public override void _EnterTree()
	{
		Instance = this;

		Config = new();
		DefaultConfig = new();
		// Not ideal
		_playerCameraSettings = ResourceLoader.Load<PC_Settings>("res://config/player/player_camera_settings.tres");

		DefaultConfig.Load(DefaultSettingsFilePath);

		if (FileAccess.FileExists(SettingsFilePath))
			Config.Load(SettingsFilePath);
		else
			Config.Load(DefaultSettingsFilePath);
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

	public static void ResetAll() =>
		Instance.Config.Load(DefaultSettingsFilePath);

	public static void ResetApply()
	{
		ResetAll();
		Apply();
	}

	public static void Reset(string section, string key)
	{
		var value = Instance.DefaultConfig.GetValue(section, key);
		Instance.Config.SetValue(section, key, value);
	}

	public static bool IsDefault(string section, string key)
	{
		var defaultValue = Instance.DefaultConfig.GetValue(section, key);
		var value = Instance.Config.GetValue(section, key);

		return defaultValue.Equals(value);
	}

	public static void Load()
	{
		//Instance.LoadVideoSettings();
		//Instance.LoadSoundSettings();
		//Instance.LoadControlSettings();
		//Instance.LoadAccessibilitySettings();

		//foreach (string sectionKey in Instance.Config.GetSections())
			foreach (string settingKey in Instance.Config.GetSectionKeys("accessibility"))
			{
				var value = Instance.Config.GetValue("accessibility", settingKey);
				if (!UserSettingsServer.Instance.TryLoadSetting("accessibility", settingKey, value, out Variant prevValue))
					GD.PrintErr(
						"Could not load config value for:" +
						"\n- section: " + "accessibility" +
						"\n - setting:" + settingKey +
						"Value remained: " + prevValue
					);
			}
	}
}
