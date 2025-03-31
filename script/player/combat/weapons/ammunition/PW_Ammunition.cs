using System;
using Godot;

public delegate void AmmutionEvent(int amount, uint finalAmount);

[GlobalClass]
public partial class PW_Ammunition : Resource
{
    [Export] private uint _magazineSize;
    /// <summary>
    /// The amount of ammunitions filled from one magazine pickup. Leave 0 for _magazineSize.
    /// Typically, there might be weapons with no magazine, and lots of ammunitions, but we don't want to load the entire ammunitions from one "magazine pickup".
    /// </summary>
    [Export] private uint _magazinePick;
    [Export] private uint _maxMagazines;
    [Export] private float _reloadTime;
    [Export] private float _tacticalReloadTime;
    public bool IsReloading {get; private set;} = false;
    private uint _maxAmos;
    private uint _unloadedAmmos;
    private uint _loadedAmmos;

    public uint UnloadedAmos
    {
        get => _unloadedAmmos;
        private set
        {
            
            int difference = value > _unloadedAmmos
                            ? (int) (value - _unloadedAmmos)
                            : - (int) (_unloadedAmmos - value);

            _unloadedAmmos = value;
            UnloadedChanged?.Invoke(difference, UnloadedAmos);
        }
    }

    public uint LoadedAmos
    {
        get => _loadedAmmos;
        private set
        {
            int difference = value > _loadedAmmos
                            ? (int) (value - _loadedAmmos)
                            : - (int) (_loadedAmmos - value);

            _loadedAmmos = value;
            LoadedChanged?.Invoke(difference, LoadedAmos);
        }
    }

    private SceneTree _sceneTree;
    private SceneTreeTimer _reloadTimer;

    public EventHandler ReloadCompleted;
    public AmmutionEvent LoadedChanged;
    public AmmutionEvent UnloadedChanged;

    /// <summary>
    /// Initialize the ammutions.
    /// </summary>
    /// <param name="baseAmos">The starting amount of ammutions.</param>
    /// <param name="load">true if the maximum amount of amos possible should be preloaded. false otherwise.</param>
    public void Initialize(SceneTree sceneTree, uint baseAmos, bool load = true)
    {
        _sceneTree = sceneTree;
        _maxAmos = _maxMagazines * _magazineSize;
        uint ammos = Math.Min(baseAmos, _maxAmos);
        if (load)
            LoadedAmos = Math.Min(_magazineSize, ammos);
        else
            LoadedAmos = 0;

        UnloadedAmos = ammos - LoadedAmos;
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

    private void Log() => GD.Print(LoadedAmos + " mag : " + UnloadedAmos/Math.Max(_magazineSize,1) + " (" + UnloadedAmos + ")");


    /// <summary>
    /// Consume the currently loaded amos and returns wether or not it did consume any of the requested amount.
    /// <para>A null amount to consumed is always considered successfull i.e. DidConsume(0) will always return true.</para>
    /// </summary>
    /// <param name="amos">The amount to consume.</param>
    /// <returns>true if the operation did consume some of the requested amos, false otherwise.</returns>
    public bool DidConsume(uint amos)
    {
        Log();
        if (IsReloading)
            return false;

        if (amos == 0)
            return true;
        
        if (LoadedAmos == 0)
            return false;
        
        LoadedAmos -= amos;
        return true;
    }

    /// <summary>
    /// Only consumes the given amount if it does have enough loaded amos.
    /// </summary>
    /// <param name="amos">The amount to consume.</param>
    /// <returns>true if it did consume, false otherwise.</returns>
    public bool TryConsume(uint amos)
    {
        Log();
        if (IsReloading)
            return false;

        if (amos > LoadedAmos)
            return false;
        
        LoadedAmos -= amos;
        return true;
    }

    public bool CanReload() => LoadedAmos < _magazineSize && UnloadedAmos > 0;

    /// <summary>
    /// Reloads the loaded amos with the unloaded amos.
    /// </summary>
    /// <returns>The amos reloaded. 0 if there's no unloaded amos left.</returns>
    public uint Reload()
    {
        uint reloaded = Math.Min(_magazineSize - LoadedAmos, UnloadedAmos);
        UnloadedAmos -= reloaded;
        LoadedAmos += reloaded;
        IsReloading = false;
        ReloadCompleted?.Invoke(this, EventArgs.Empty);
        Log();
        return reloaded;
    }

    /// <summary>
    /// Fill in the unloaded amos.
    /// </summary>
    /// <param name="amount">The amount to fill.</param>
    /// <param name="magazine">If the amount is pure ammos, or pickup magazines.</param>
    /// <returns>The amos really filled. 0 if the maximum amount of amos is already reached.</returns>
    public uint FillAmos(uint amount, bool magazine)
    {
        uint amos = magazine ? AmmosFromPickup(amount) : amount;
        uint filled = Math.Min(amos, _maxAmos - UnloadedAmos - _magazineSize);
        UnloadedAmos += filled;
        Log();
        return filled;
    }

    /// <summary>
    /// Empty from the unloaded amos.
    /// </summary>
    /// <param name="amount">The amount to empty.</param>
    /// <param name="magazine">If the amount is pure ammos, or pickup magazines.</param>
    /// <returns>The amos really emptied. 0 if the unloaded amos were already 0.</returns>
    public uint EmptyAmos(uint amount, bool magazine)
    {
        uint amos = magazine ? AmmosFromPickup(amount) : amount;
        uint emptied = Math.Min(amos, UnloadedAmos);
        UnloadedAmos -= emptied;
        Log();
        return emptied;
    }

    /// <summary>
    /// A short hand for the sum of Unloaded and Loaded amos.
    /// </summary>
    /// <returns>The total amos currently possessed.</returns>
    public uint TotalAmos() => UnloadedAmos + LoadedAmos;

    /// <summary>
    /// A quick way to access the amount of munitions gathered from `amount` pickup magazines.
    /// </summary>
    /// <param name="amount">the amount of pickup magazines.</param>
    /// <returns>The amount of ammunitions.</returns>
    public uint AmmosFromPickup(uint amount) => (_magazinePick == 0 ? _magazineSize : _magazinePick) * amount;
}