public class GL_SlowMoPickHandler(GL_SlowMoData data) : GL_IPickHandler
{
    private readonly GL_SlowMoData _data = data;
    public bool HandlePick(GL_Picker picker)
    {
        GL_SlowMoProcess process = _data.SlowMoProcess.Instantiate<GL_SlowMoProcess>();
        process.InitData(_data);
        picker.AddChild(process);
        picker.PickSlowMo(_data);
        return true;
    }
}