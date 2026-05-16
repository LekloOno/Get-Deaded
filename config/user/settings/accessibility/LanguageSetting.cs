using Godot;
using TraGUS;

public partial class LanguageSetting : UserSetting
{
    public override string Section => UserSettingsSection.Accessibility;
    public override string Key => "language";

    public override Variant DefaultFallBack() => "en";

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType != Variant.Type.String)
        {
            effectiveValue = Value;
            return false;
        }

        string locale = (string) value;
        if (!TranslationServer.HasTranslationForLocale(locale, true))
        {
            effectiveValue = value;
            return false;
        }

        effectiveValue = value;
        TranslationServer.SetLocale(locale);

        return true;
    }
}