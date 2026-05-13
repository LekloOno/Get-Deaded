using Godot;

public partial class CONF_UserSettingsLoader : Node
{
    public static Variant GetControlSetting(string key) =>
		Instance.Config.GetValue("control", key);

	public static void RegisterControlSetting(string key, Variant value) =>
		Instance.Config.SetValue("control", key, value);
        
    private void LoadControlSettings()
	{
		foreach (string key in Config.GetSectionKeys("control"))
		{
			var value = Config.GetValue("control", key);

			switch (key)
			{
				case "cm_per_360" :
					_playerCameraSettings.CmPer360 = (float)value;
					break;

				case "dpi" :
					_playerCameraSettings.Dpi = (uint)value;
					break;
			}
		}
	}
}