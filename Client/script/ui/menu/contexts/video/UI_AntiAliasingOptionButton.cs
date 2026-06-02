using System;
using Godot;

[GlobalClass]
public partial class UI_AntiAliasingOptionButton : OptionButton
{
    public override void _Ready()
    {
        Populate();

        SetAntiAliasing(AntiAliasingSetting.Method);
        AntiAliasingSetting.Instance.Changed += OnSettingChanged;
        
        ItemSelected += OnItemSelected;
    }

    private void OnSettingChanged(GodotObject sender, Variant value)
    {
        if (sender == this)
            return;

        SetAntiAliasing((AntiAliasing) (int) value);
    }

    private void Populate()
    {
        Clear();

        foreach (AntiAliasing aliasing in Enum.GetValues<AntiAliasing>())
        {
            int index = ItemCount;

            AddItem($"{aliasing.ToString().ToUpperInvariant()}");
            SetItemMetadata(index, (int)aliasing);
        }
    }

    private void SetAntiAliasing(AntiAliasing aliasing)
    {
        for (int i = 0; i < ItemCount; i++)
        {
            var itemAliasing = (AntiAliasing)(int)GetItemMetadata(i);

            if (itemAliasing == aliasing)
            {
                Select(i);
                return;
            }
        }

        GD.PushWarning($"[UI_AntiAliasingOptionButton] Method {aliasing} is not available in this OptionButton.");
    }

    private void OnItemSelected(long index)
    {
        var method = (AntiAliasing)(int)GetItemMetadata((int)index);
        
        if (!AntiAliasingSetting.Instance.TryUpdateValue(this, (int) method, out Variant effectiveValue))
            SetAntiAliasing((AntiAliasing) (int) effectiveValue);
    }
}