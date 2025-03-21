using Godot;

public partial class PC_SimpleRecoil(PC_Recoil recoilController) : PC_RecoilHandler(recoilController)
{
    protected override void OnThresholdReached() => SetProcess(false);
    protected override void CustomProcess(){}
    protected override void CustomAdd(Vector2 velocity){}
    protected override void CustomAddCapped(Vector2 velocity, float max){}
}