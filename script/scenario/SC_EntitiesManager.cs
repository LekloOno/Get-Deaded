using System.Collections.Generic;

public static class SC_EntitiesManager
{
    private static List<PickableSpawner> _pickupSpawners = [];

    public static void Register(PickableSpawner spawner) =>
        _pickupSpawners.Add(spawner);

    public static void Unregister(PickableSpawner spawner) =>
        _pickupSpawners.Remove(spawner);

    public static void EnablePickups()
    {
        foreach (PickableSpawner spawner in _pickupSpawners)
            spawner.Enable();
    }

    public static void DisablePickups()
    {
        foreach (PickableSpawner spawner in _pickupSpawners)
            spawner.Disable();
    }
}