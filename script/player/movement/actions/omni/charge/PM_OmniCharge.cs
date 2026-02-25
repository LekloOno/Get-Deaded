using Godot;

[GlobalClass, Tool]
public partial class PM_OmniCharge : Node
{
    [Export] public float Max {get; private set;}
    
    public float Current
    {
        get => _current;
        set {
           _current = Mathf.Clamp(value, 0, Max);
        }
    }
    
    private float _current;
    private PM_IOmniLoader _loader;

    public override void _Ready()
    {
        if (Engine.IsEditorHint())
            return;

        _current = Max;
    }

    public bool TryConsume(float charge)
    {
        if (charge > Current)
        {
            _loader.TriedConsume(charge);
            return false;
        }

        Current -= charge;
        _loader.Consumed(charge);
        return true;
    }


    // +-----------------+
    // |  CONFIGURATION  |
    // +-----------------+
    // ____________________
    public override void _Notification(int what)
    {
        if (what != NotificationChildOrderChanged)
            return;
        
        _loader = GetLoader();
        if (_loader != null)
            _loader.Charge = this;
    }

    private PM_IOmniLoader GetLoader()
    {
        foreach(Node node in GetChildren())
            if (node is PM_IOmniLoader l)
                return l;
        return null;
    }
}