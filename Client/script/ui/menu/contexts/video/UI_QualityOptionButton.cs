using System;
using Godot;
using TraGUS;

[GlobalClass]
public partial class UI_QualityOptionButton : OptionButton
{
    [Export(PropertyHint.Flags)]
    private VideoQualityMask _allowedQualities =
        VideoQualityMask.Minimal |
        VideoQualityMask.Low |
        VideoQualityMask.Medium |
        VideoQualityMask.High |
        VideoQualityMask.Ultra;

    [Export]
    private string? _settingKey;

    private UserSetting? _userSetting;

    public override void _Ready()
    {
        if (_settingKey == null)
        {
            GD.PrintErr("[UI_QualityOptionButton] Missing _userSetting.");
            return;
        }

        UserSettingsServer.GetSetting("video", _settingKey, out _userSetting);
        
        if (_userSetting == null)
        {
            GD.PrintErr($"[UI_QualityOptionButton] UserSettingsServer : '{_userSetting}' is not a valid registered setting key.");
            return;
        }

        Populate();

        SetQuality((VideoQuality) (int) _userSetting.Value);
        _userSetting.Changed += OnSettingChanged;
        
        ItemSelected += OnItemSelected;
    }

    private void OnSettingChanged(GodotObject sender, Variant value)
    {
        if (sender == this)
            return;

        SetQuality((VideoQuality) (int) value);
    }

    private void Populate()
    {
        Clear();

        foreach (VideoQuality quality in Enum.GetValues<VideoQuality>())
        {
            var mask = quality.ToMask();

            if (!_allowedQualities.HasFlag(mask))
                continue;

            int index = ItemCount;

            AddItem($"{quality.ToString().ToUpperInvariant()}_QUALITY_OPTION");
            SetItemMetadata(index, (int)quality);
        }
    }

    private void SetQuality(VideoQuality quality)
    {
        for (int i = 0; i < ItemCount; i++)
        {
            var itemQuality = (VideoQuality)(int)GetItemMetadata(i);

            if (itemQuality == quality)
            {
                Select(i);
                return;
            }
        }

        GD.PushWarning($"[UI_QualityOptionButton] Quality {quality} is not available in this OptionButton.");
    }

    private void OnItemSelected(long index)
    {
        if (_userSetting == null)
            return;

        var quality = (VideoQuality)(int)GetItemMetadata((int)index);
        
        if (!_userSetting.TryUpdateValue(this, (int) quality, out Variant effectiveValue))
            SetQuality((VideoQuality) (int) effectiveValue);
    }
}