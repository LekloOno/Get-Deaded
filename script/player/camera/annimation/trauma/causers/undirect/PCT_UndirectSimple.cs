using Godot;

namespace Pew;

public partial class PCT_UndirectSimple : PCT_Undirect
{
    [Export] protected float _amount = 0.1f;
    
    protected override float ProcessedAmount(PC_Shakeable shakeable) => _amount;
}