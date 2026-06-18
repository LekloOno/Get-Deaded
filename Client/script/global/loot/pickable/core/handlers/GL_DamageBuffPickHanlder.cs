public class GL_DamageBuffPickHandler(GL_DamageBuffData data) : GL_IPickHandler
{
    private readonly GL_DamageBuffData _data = data;
    public bool HandlePick(GL_Picker picker)
    {
        picker.PickDamageMultiplier(_data);
        return true;
    }
}