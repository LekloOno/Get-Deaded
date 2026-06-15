using Godot;

[GlobalClass]
public partial class DATA_Slam : Resource
{
    public DATA_Slam()
    {
        SetDashSpeed();    
    }

    [Export] public float ChargeCost        { get; private set; } = 66.7f;
    private float _distance = 6.5f;
    private float _duration = 0.09f;
    public float Speed                  { get; private set; }

    [Export(PropertyHint.Range, "0.0, 10.0, or_greater")] public float Distance
    {
        get => _distance;
        set
        {
            _distance = value;
            SetDashSpeed();
        }
    }
    [Export(PropertyHint.Range, "0.01, 0.5, or_greater")]
    public float Duration
    {
        get => _duration;
        set
        {
            _duration = value;
            SetDashSpeed();
        }
    }

    private void SetDashSpeed() =>
        Speed = _distance/_duration;
}