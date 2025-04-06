using Godot;

[GlobalClass]
public partial class PC_ScalableTraumaCauser : PC_TraumaCauser
{
    [Export] private float _minimumAmount = 0.15f;
    [Export] private float _minDistance = 8f;       // Distance before the trauma starts decaying
    [Export] private float _maxDistance = 40f;      // Distance at which the trauma amount reaches _minimumAmount; 
    protected override float ProcessedAmount(PC_Shakeable shakeable)
    {
        float distanceRatio = (shakeable.GlobalPosition.DistanceTo(GlobalPosition) - _minDistance) / (_maxDistance - _minDistance);
        distanceRatio = Mathf.Clamp(distanceRatio, 0f, 1f);
        return Mathf.Lerp(_amount, _minimumAmount, distanceRatio);
    }
}