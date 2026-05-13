using Godot;

public partial class CONF_UserSettingsLoader : Node
{
    public static Variant GetVideoSetting(string key) =>
		Instance.Config.GetValue("video", key);

	public static void RegisterVideoSetting(string key, Variant value) =>
		Instance.Config.SetValue("video", key, value);
        
    private void LoadVideoSettings()
	{
		foreach (string key in Config.GetSectionKeys("video"))
		{
			var value = Config.GetValue("video", key);
			switch (key)
			{
				case "display_mode" :
					DisplayServer.WindowSetMode((DisplayServer.WindowMode)(int)value);
					break;

				case "resolution" :
					Vector2I res = (Vector2I)value;
					if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Windowed)
						GetWindow().Size = res;
					GetWindow().ContentScaleSize = res;
					break;

				case "stretch_mode" :
					GetTree().Root.ContentScaleAspect = (Window.ContentScaleAspectEnum)(int)value;
					break;

				case "render_scale_mode" :
					GetTree().Root.Scaling3DMode = (Viewport.Scaling3DModeEnum)(int)value;
					break;

				case "render_scale_scale" :
					GetTree().Root.Scaling3DScale = (float)value;
					break;

				case "fsr_sharpness" :
					GetTree().Root.FsrSharpness = (float)value;
					break;

				case "fov" :
					_playerCameraSettings.HorizontalFov = (float)value;
					break;

				case "vsync_mode" :
					DisplayServer.WindowSetVsyncMode((DisplayServer.VSyncMode)(int)value);
					break;

				case "max_fps" :
					Engine.MaxFps = (int)value;
					break;
			}
		}
	}
}