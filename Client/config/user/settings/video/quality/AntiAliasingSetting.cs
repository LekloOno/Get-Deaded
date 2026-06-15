using System;
using Godot;
using TraGUS.DotNet.Conversion;

public partial class AntiAliasingSetting : UserSettingEnum<AntiAliasingSetting, AntiAliasing>
{
    public const string KeyString = "anti_aliasing";
    public override string Section => UserSettingsSection.Video;
    public override string Key => KeyString;

    public override Variant DefaultFallBack() => (int) AntiAliasing.Disabled;
    public static AntiAliasing Method => Tval;

    protected override bool ProcessTypedValue(AntiAliasing typedValue, out AntiAliasing effectiveTypedValue)
    {
        effectiveTypedValue = typedValue;
        UpdateFrom(Method);

        return true;
    }

    protected void UpdateFrom(AntiAliasing quality)
    {
        Rid? vpRid = GetViewport()?.GetViewportRid();
        if (vpRid is not Rid rid)
            return;

        RenderingServer.ViewportSetMsaa3D(rid, RenderingServer.ViewportMsaa.Disabled);
        RenderingServer.ViewportSetScreenSpaceAA(rid, RenderingServer.ViewportScreenSpaceAA.Disabled);
        RenderingServer.ViewportSetUseTaa(rid, false);

        switch (quality)
        {
            case AntiAliasing.Disabled :
                break;

            case AntiAliasing.Msaa2x :
                RenderingServer.ViewportSetMsaa3D(rid, RenderingServer.ViewportMsaa.Msaa2X);
                break;
            
            case AntiAliasing.Msaa4x :
                RenderingServer.ViewportSetMsaa3D(rid, RenderingServer.ViewportMsaa.Msaa4X);
                break;
            
            case AntiAliasing.Msaa8x :
                RenderingServer.ViewportSetMsaa3D(rid, RenderingServer.ViewportMsaa.Msaa8X);
                break;

            case AntiAliasing.Fxaa :
                RenderingServer.ViewportSetScreenSpaceAA(rid, RenderingServer.ViewportScreenSpaceAA.Fxaa);
                break;

            case AntiAliasing.Smaa :
                RenderingServer.ViewportSetScreenSpaceAA(rid, RenderingServer.ViewportScreenSpaceAA.Smaa);
                break;

            case AntiAliasing.Taa :
                RenderingServer.ViewportSetUseTaa(rid, true);
                break;
        }
    }
}

public enum AntiAliasing
{
    Disabled,
    Msaa2x,
    Msaa4x,
    Msaa8x,
    Fxaa,
    Smaa,
    Taa,
}