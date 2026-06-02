using System;
using Godot;
using TraGUS;

[GlobalClass]
public partial class UI_QualityPresetOptionButton : OptionButton
{
    const int CustomId = -1;

    private UserSetting? _antiAliasing;

    private UserSetting? _ssao;
    private UserSetting? _ssil;
    private UserSetting? _ssr;
    private UserSetting? _glow;

    private UserSetting? _dirShadows;
    private UserSetting? _posShadows;

    public override void _Ready()
    {
        Populate();

        Select(ItemCount - 1);

        UserSettingsServer.GetSetting(UserSettingsSection.Video, AntiAliasingSetting.KeyString, out _antiAliasing);

        UserSettingsServer.GetSetting(UserSettingsSection.Video, AmbientOcclusionSetting.KeyString, out _ssao);
        UserSettingsServer.GetSetting(UserSettingsSection.Video, SsilSetting.KeyString, out _ssil);
        UserSettingsServer.GetSetting(UserSettingsSection.Video, SsrSetting.KeyString, out _ssr);
        UserSettingsServer.GetSetting(UserSettingsSection.Video, GlowSetting.KeyString, out _glow);

        UserSettingsServer.GetSetting(UserSettingsSection.Video, DirectionalShadowsSetting.KeyString, out _dirShadows);
        UserSettingsServer.GetSetting(UserSettingsSection.Video, PositionalShadowsSetting.KeyString, out _posShadows);

        _antiAliasing.Changed += OnSettingChanged;

        _ssao.Changed += OnSettingChanged;
        _ssil.Changed += OnSettingChanged;
        _ssr.Changed += OnSettingChanged;
        _glow.Changed += OnSettingChanged;

        _dirShadows.Changed += OnSettingChanged;
        _posShadows.Changed += OnSettingChanged;
        
        ItemSelected += OnItemSelected;
    }

    private void OnSettingChanged2(GodotObject sender, Variant value) => OnSettingChanged(sender, value);

    private void OnSettingChanged(GodotObject sender, Variant value)
    {
        if (sender == this)
            return;

        Select(ItemCount - 1);
    }

    private void Populate()
    {
        Clear();

        foreach (VideoQuality quality in Enum.GetValues<VideoQuality>())
        {
            var mask = quality.ToMask();

            int index = ItemCount;

            if (quality == VideoQuality.Disabled)
                AddItem("POTATOE_QUALITY_OPTION");
            else
                AddItem($"{quality.ToString().ToUpperInvariant()}_QUALITY_OPTION");

            SetItemMetadata(index, (int)quality);
        }

        AddItem("CUSTOM_QUALITY_OPTION", CustomId);
        int customIndex = ItemCount - 1;
        SetItemDisabled(customIndex, true);
    }

    private void OnItemSelected(long index)
    {
        var preset = (int)GetItemMetadata((int)index);

        if (preset == CustomId)
            return;

        UpdateFromQuality((VideoQuality) preset);
    }

    private void UpdateFromQuality(VideoQuality quality)
    {
        _antiAliasing?.TryUpdateValue(this, (int) FromQuality(quality), out _);
        
        int intQuality = (int) quality;
        _ssao?.TryUpdateValue(this, intQuality, out _);

        if (quality == VideoQuality.Minimal || quality == VideoQuality.Low)
        {
            _ssil?.TryUpdateValue(this, (int) VideoQuality.Disabled, out _);
            _ssr?.TryUpdateValue(this, (int) VideoQuality.Disabled, out _);
        }
        else
        {
            _ssil?.TryUpdateValue(this, intQuality, out _);
            _ssr?.TryUpdateValue(this, intQuality, out _);
        }
        
        _glow?.TryUpdateValue(this, intQuality, out _);
        
        _dirShadows?.TryUpdateValue(this, intQuality, out _);
        _posShadows?.TryUpdateValue(this, intQuality, out _);
    }

    private AntiAliasing FromQuality(VideoQuality quality)
    {
        return quality switch
        {
            VideoQuality.Disabled => AntiAliasing.Disabled,
            _ => AntiAliasing.Smaa,
        };
    }
}