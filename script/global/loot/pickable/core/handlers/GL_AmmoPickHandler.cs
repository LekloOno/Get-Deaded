namespace Pew;

public class GL_AmmoPickHandler(GL_AmmoData data) : GL_IPickHandler
{
    private GL_AmmoData _data = data;
    public bool HandlePick(GL_Picker picker)
    {
        return picker.PickAmmo(_data);
    }
}