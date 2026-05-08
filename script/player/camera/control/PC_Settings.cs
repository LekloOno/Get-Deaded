using Godot;

[GlobalClass]
public partial class PC_Settings : Resource
{
    [Export(PropertyHint.Range, "0,100,0.1")]
    public float CmPer360 = 32f;

    [Export(PropertyHint.Range, "0,64000,1")]
    public uint Dpi = 1600;
    public float Sensitivity => 5.08f * Mathf.Pi
                                / (Dpi * CmPer360);
    public Observable<float> Fov = new Observable<float>(115f);

    [Export] public float HorizontalFov
    {
        get => Fov;
        set => Fov.Value = value;
    }
}