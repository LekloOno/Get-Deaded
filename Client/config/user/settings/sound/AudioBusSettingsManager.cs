using System.Collections.Generic;
using Godot;

public partial class AudioBusSettingsManager : Node
{
    public AudioBusSettingsManager Instance {get; private set;}
    private static Dictionary<string, AudioBusSetting> _buses = [];

    [Signal]
    public delegate void BusesInitializedEventHandler();
    public bool IsInitialized {get; private set;} = false;

    public override void _EnterTree()
    {
        Instance = this;
        
        InitializeBuses();
    }

    private async void InitializeBuses()
    {
        // Not necessary async, but keep it in the back of my hand in case of ..
        // I thought for a moment while debugging a weird bug on audio busses that
        // audio busses might be initialized too late in the game export, where in editor it worked well
        // The actual problem seemed to come from a weird parsing error instead, that I """"fixed"""" with
        // an explicit float getter of the property.
        
        // Not that as is, setting this async defer breaks the reset buttons ..
        //await ToSignal(GetTree(), SceneTree.SignalName.PhysicsFrame);
        _buses.Clear();

        int count = AudioServer.GetBusCount();

        for (int i = 0; i < count; i++)
        {
            string busName = AudioServer.GetBusName(i);
            AddBus(busName, i);
        }

        IsInitialized = true;
        EmitSignal(SignalName.BusesInitialized);
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