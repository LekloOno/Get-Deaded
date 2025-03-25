public class GL_AmmoPickHandler : GL_IPickHandler
{
    public bool HandlePick(GL_Picker picker)
    {
        return picker.PickAmmo();
    }
}