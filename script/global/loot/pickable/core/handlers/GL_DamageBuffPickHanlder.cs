public class GL_DamageBuffPickHandler(GL_DamageBuffData data) : GL_IPickHandler
{
    private GL_DamageBuffData _data = data;
    public bool HandlePick(GL_Picker picker)
    {
        picker.PickDamageMultiplier(_data);
        return true;
    }
}