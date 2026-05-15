using Godot;

public partial class CONF_UserSettingsLoader : Node
{
    public const string VideoSection = "video";

	public Observable<Variant> DisplayMode = new(0);

    public static class VideoSetting
    {
        public const string DisplayMode = "display_mode";
        public const string Resolution = "resolution";
        public const string StrechMode = "stretch_mode";
        public const string RenderScaleMode = "render_scale_mode";
        public const string RenderScaleScale = "render_scale_scale";
        public const string FsrSharpness = "fsr_sharpness";
        public const string Fov = "fov";
        public const string VSyncMode = "vsync_mode";
        public const string MaxFps = "max_fps";
    }

    public static Variant GetVideoSetting(string key) =>
		Instance.Config.GetValue(VideoSection, key);

	public static void RegisterVideoSetting(string key, Variant value) =>
		Instance.Config.SetValue(VideoSection, key, value);
        
    private void LoadVideoSettings()
	{
		foreach (string key in Config.GetSectionKeys(VideoSection))
		{
			var value = Config.GetValue(VideoSection, key);
			switch (key)
			{
				case VideoSetting.DisplayMode:
					DisplayServer.WindowSetMode((DisplayServer.WindowMode)(int)value);
					break;

				case VideoSetting.Resolution :
					Vector2I res = (Vector2I)value;
					if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Windowed)
						GetWindow().Size = res;
					GetWindow().ContentScaleSize = res;
					break;

				case VideoSetting.StrechMode :
					GetTree().Root.ContentScaleAspect = (Window.ContentScaleAspectEnum)(int)value;
					break;

				case VideoSetting.RenderScaleMode :
					GetTree().Root.Scaling3DMode = (Viewport.Scaling3DModeEnum)(int)value;
					break;

				case VideoSetting.RenderScaleScale :
					GetTree().Root.Scaling3DScale = (float)value;
					break;

				case VideoSetting.FsrSharpness :
					GetTree().Root.FsrSharpness = (float)value;
					break;

				case VideoSetting.Fov :
					_playerCameraSettings.HorizontalFov = (float)value;
					break;

				case VideoSetting.VSyncMode :
					DisplayServer.WindowSetVsyncMode((DisplayServer.VSyncMode)(int)value);
					break;

				case VideoSetting.MaxFps :
					Engine.MaxFps = (int)value;
					break;
			}
		}
	}
}