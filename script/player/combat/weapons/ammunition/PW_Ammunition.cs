using System;
using Godot;

public delegate void AmmunitionEvent(int amount, uint finalAmount);

/// <summary>
/// Handles Weapons Ammunitions.
/// </summary>

// Icon credits - LekloOno - https://github.com/LekloOno
[GlobalClass, Icon("res://gd_icons/weapon_system/Ammos_icon.svg")]
public partial class PW_Ammunition : WeaponComponent
{
    [Export] private uint _magazineSize;
    /// <summary>
    /// The amount of ammunitions filled from one magazine pickup. Leave 0 for _magazineSize.
    /// Typically, there might be weapons with no magazine, and lots of ammunitions, but we don't want to load the entire ammunitions from one "magazine pickup".
    /// </summary>
    [Export] private uint _magazinePick;
    [Export] private uint _maxMagazines;
    [Export] protected uint _baseAmmos;
    public bool IsReloading {get; private set;} = false;
    private uint _maxAmmos;
    private uint _unloadedAmmos;
    private uint _loadedAmmos;

    public EventHandler ReloadCompleted;
    public AmmunitionEvent LoadedChanged;
    public AmmunitionEvent UnloadedChanged;

    public uint UnloadedAmmos
    {
        get => _unloadedAmmos;
        private set
        {
            
            int difference = value > _unloadedAmmos
                            ? (int) (value - _unloadedAmmos)
                            : - (int) (_unloadedAmmos - value);

            _unloadedAmmos = value;
            UnloadedChanged?.Invoke(difference, UnloadedAmmos);
        }
    }

    public uint LoadedAmmos
    {
        get => _loadedAmmos;
        private set
        {
            int difference = value > _loadedAmmos
                            ? (int) (value - _loadedAmmos)
                            : - (int) (_loadedAmmos - value);

            _loadedAmmos = value;
            LoadedChanged?.Invoke(difference, LoadedAmmos);
        }
    }

    /// <summary>
    /// Initialize the ammutions.
    /// </summary>
    /// <param name="baseAmmos">The starting amount of ammutions.</param>
    /// <param name="load">true if the maximum amount of ammos possible should be preloaded. false otherwise.</param>
    public void Initialize(bool load = true)
    {
        _maxAmmos = _maxMagazines * _magazineSize;
        uint ammos = Math.Min(_baseAmmos, _maxAmmos);
        if (load)
            LoadedAmmos = Math.Min(_magazineSize, ammos);
        else
            LoadedAmmos = 0;

        UnloadedAmmos = ammos - LoadedAmmos;
    }

    /// <summary>
    /// Consume the currently loaded ammos.
    /// </summary>
    /// <param name="ammos">The amount to consume.</param>
    /// <returns>The ammos really consumed. 0 if there's no loaded ammos left.</returns>
    public uint Consume(uint ammos)
    {
        uint consumed = Math.Min(ammos, LoadedAmmos);
        LoadedAmmos -= consumed;
        return consumed;
    }

    private void Log() => GD.Print(LoadedAmmos + " mag : " + UnloadedAmmos/Math.Max(_magazineSize,1) + " (" + UnloadedAmmos + ")");


    /// <summary>
    /// Consume the currently loaded ammos and returns wether or not it did consume any of the requested amount.
    /// <para>A null amount to consumed is always considered successfull i.e. DidConsume(0) will always return true.</para>
    /// </summary>
    /// <param name="ammos">The amount to consume.</param>
    /// <returns>true if the operation did consume some of the requested ammos, false otherwise.</returns>
    public bool DidConsume(uint ammos)
    {
        if (IsReloading)
            return false;

        if (ammos == 0)
            return true;
        
        if (LoadedAmmos == 0)
            return false;
        
        LoadedAmmos -= ammos;
        return true;
    }

    /// <summary>
    /// Only consumes the given amount if it does have enough loaded ammos.
    /// </summary>
    /// <param name="ammos">The amount to consume.</param>
    /// <returns>true if it did consume, false otherwise.</returns>
    public bool TryConsume(uint ammos)
    {
        if (IsReloading)
            return false;

        if (ammos > LoadedAmmos)
            return false;
        
        LoadedAmmos -= ammos;
        return true;
    }

    public bool CanReload() => LoadedAmmos < _magazineSize && UnloadedAmmos > 0;

    /// <summary>
    /// Reloads the loaded ammos with the unloaded ammos.
    /// </summary>
    /// <returns>The ammos reloaded. 0 if there's no unloaded ammos left.</returns>
    public uint Reload()
    {
        uint reloaded = Math.Min(_magazineSize - LoadedAmmos, UnloadedAmmos);
        UnloadedAmmos -= reloaded;
        LoadedAmmos += reloaded;
        IsReloading = false;
        ReloadCompleted?.Invoke(this, EventArgs.Empty);
        return reloaded;
    }

    /// <summary>
    /// Fill in the unloaded ammos.
    /// </summary>
    /// <param name="amount">The amount to fill.</param>
    /// <param name="magazine">If the amount is pure ammos, or pickup magazines.</param>
    /// <returns>The ammos really filled. 0 if the maximum amount of ammos is already reached.</returns>
    public uint FillAmmos(uint amount, bool magazine)
    {
        uint ammos = magazine ? AmmosFromPickup(amount) : amount;
        uint filled = Math.Min(ammos, _maxAmmos - UnloadedAmmos - _magazineSize);
        UnloadedAmmos += filled;
        return filled;
    }

    /// <summary>
    /// Empty from the unloaded ammos.
    /// </summary>
    /// <param name="amount">The amount to empty.</param>
    /// <param name="magazine">If the amount is pure ammos, or pickup magazines.</param>
    /// <returns>The ammos really emptied. 0 if the unloaded ammos were already 0.</returns>
    public uint EmptyAmmos(uint amount, bool magazine)
    {
        uint ammos = magazine ? AmmosFromPickup(amount) : amount;
        uint emptied = Math.Min(ammos, UnloadedAmmos);
        UnloadedAmmos -= emptied;
        return emptied;
    }

    /// <summary>
    /// A short hand for the sum of Unloaded and Loaded ammos.
    /// </summary>
    /// <returns>The total ammos currently possessed.</returns>
    public uint TotalAmmos() => UnloadedAmmos + LoadedAmmos;

    /// <summary>
    /// A quick way to access the amount of munitions gathered from `amount` pickup magazines.
    /// </summary>
    /// <param name="amount">the amount of pickup magazines.</param>
    /// <returns>The amount of ammunitions.</returns>
    public uint AmmosFromPickup(uint amount) => (_magazinePick == 0 ? _magazineSize : _magazinePick) * amount;
}