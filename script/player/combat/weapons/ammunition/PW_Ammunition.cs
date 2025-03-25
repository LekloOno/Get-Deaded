using System;
using Godot;

public partial class PW_Ammunition : Resource
{
    [Export] private uint _magazineSize;
    [Export] private uint _maxMagazines;
    private uint _maxAmos;
    public uint UnloadedAmos {get; private set;}
    public uint LoadedAmos {get; private set;}

    /// <summary>
    /// Initialize the ammutions.
    /// </summary>
    /// <param name="baseAmos">The starting amount of ammutions.</param>
    /// <param name="load">true if the maximum amount of amos possible should be preloaded. false otherwise.</param>
    public void Initialize(uint baseAmos, bool load = true)
    {
        _maxAmos = _maxMagazines * _magazineSize;
        if (load)
            LoadedAmos = Math.Min(_magazineSize, _maxAmos);
        else
            LoadedAmos = 0;

        UnloadedAmos = _maxAmos - LoadedAmos;
    }

    /// <summary>
    /// Consume the currently loaded amos.
    /// </summary>
    /// <param name="amos">The amount to consume.</param>
    /// <returns>The amos really consumed. 0 if there's no loaded amos left.</returns>
    public uint Consume(uint amos)
    {
        uint consumed = Math.Min(amos, LoadedAmos);
        LoadedAmos -= consumed;
        return consumed;
    }

    /// <summary>
    /// Reloads the loaded amos with the unloaded amos.
    /// </summary>
    /// <returns>The amos reloaded. 0 if there's no unloaded amos left.</returns>
    public uint Reload()
    {
        uint reloaded = Math.Min(_magazineSize - LoadedAmos, UnloadedAmos);
        UnloadedAmos -= reloaded;
        LoadedAmos += reloaded;
        return reloaded;
    }

    /// <summary>
    /// Fill in the unloaded amos.
    /// </summary>
    /// <param name="amos">The amount to fill.</param>
    /// <returns>The amos really filled. 0 if the maximum amount of amos is already reached.</returns>
    public uint FillAmos(uint amos)
    {
        uint loaded = Math.Min(_maxAmos - TotalAmos(), amos);
        UnloadedAmos += amos;
        return loaded;
    }

    /// <summary>
    /// A short hand for the sum of Unloaded and Loaded amos.
    /// </summary>
    /// <returns>The total amos currently possessed.</returns>
    public uint TotalAmos() => UnloadedAmos + LoadedAmos;
}