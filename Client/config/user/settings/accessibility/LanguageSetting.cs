using Godot;
using TraGUS.DotNet.Conversion;

public partial class LanguageSetting : UserSettingString<LanguageSetting>
{
    public override string Section => UserSettingsSection.Accessibility;
    public override string Key => "language";

    public override Variant DefaultFallBack() => "en";

    protected override bool ProcessTypedValue(string typedValue, out string effectiveTypedValue)
    {
        if (!TranslationServer.HasTranslationForLocale(typedValue, true))
        {
            effectiveTypedValue = Tval;
            return false;
        }

        effectiveTypedValue = typedValue;
        TranslationServer.SetLocale(typedValue);
        return true;
    }
}