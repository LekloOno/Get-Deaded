using Godot;

public partial class CONF_UserSettingsLoader : Node
{
	const string AccessibilitySection = "accessibility";

	public static class AccessibilitySetting
    {
        public const string EnemiesColor = "enemies_color";
        public const string Language = "language";
    }

    public static Variant GetAccessibilitySetting(string key) =>
		Instance.Config.GetValue(AccessibilitySection, key);

	public static void RegisterAccessibilitySetting(string key, Variant value) =>
		Instance.Config.SetValue(AccessibilitySection, key, value);
        
    private void LoadAccessibilitySettings()
	{
		foreach (string key in Config.GetSectionKeys(AccessibilitySection))
		{
			var value = Config.GetValue(AccessibilitySection, key);

			switch (key)
			{
				case AccessibilitySetting.EnemiesColor :
					CONF_HitColors.Instance.HitColors.Critical = (Color)value;
					break;

				case AccessibilitySetting.Language :
					TranslationServer.SetLocale((string)value);
					break;
			}
		}
	}
}