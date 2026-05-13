using Godot;

public partial class CONF_UserSettingsLoader : Node
{
	const string ControlSection = "control";

	public static class ControlSetting
    {
        public const string CmPer360 = "cm_per_360";
        public const string Dpi = "dpi";
    }

    public static Variant GetControlSetting(string key) =>
		Instance.Config.GetValue(ControlSection, key);

	public static void RegisterControlSetting(string key, Variant value) =>
		Instance.Config.SetValue(ControlSection, key, value);
        
    private void LoadControlSettings()
	{
		foreach (string key in Config.GetSectionKeys(ControlSection))
		{
			var value = Config.GetValue(ControlSection, key);

			switch (key)
			{
				case ControlSetting.CmPer360 :
					_playerCameraSettings.CmPer360 = (float)value;
					break;

				case ControlSetting.Dpi :
					_playerCameraSettings.Dpi = (uint)value;
					break;
			}
		}
	}
}