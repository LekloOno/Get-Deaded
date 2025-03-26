public class GL_AmmoPickHandler(uint weaponIndex, uint fireIndex, uint ammos) : GL_IPickHandler
{
    private uint _weaponIndex = weaponIndex;
    private uint _fireIndex = fireIndex;
    private uint _ammos = ammos;

    public bool HandlePick(GL_Picker picker)
    {
        return picker.PickAmmo();
    }
}