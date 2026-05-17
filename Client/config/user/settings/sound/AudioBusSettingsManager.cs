using System.Collections.Generic;
using Godot;

public partial class AudioBusSettingsManager : Node
{
    public AudioBusSettingsManager Instance {get; private set;}
    private static Dictionary<string, AudioBusSetting> _buses = [];

    public override void _EnterTree()
    {
        Instance = this;

        int count = AudioServer.GetBusCount();
        for (int i = 0; i < count; i++)
        {
            string busName = AudioServer.GetBusName(i);
            AddBus(busName, i);
            //GD.Print(busName);
        }
    }

    public AudioBusSetting GetBus(string busName)
    {
        _buses.TryGetValue(busName, out AudioBusSetting bus);
        return bus;
    }

    private void AddBus(string name, int index)
    {
        AudioBusSetting bus = new(name, index);
        if (_buses.TryAdd(name, bus))
            AddChild(bus);
        else
            GD.PushWarning("Could not add bus setting for name `" + name + "`.");
    }
}