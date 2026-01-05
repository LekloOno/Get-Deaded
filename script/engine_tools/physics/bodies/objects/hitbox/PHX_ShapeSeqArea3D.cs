using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class PHX_ShapeSeqArea3D : PHX_SequenceArea3D
{
    record SequenceStep(CollisionShape3D Shape, Timer Timer);
    /// <summary>
    /// If a shape is found in the tree of the Area3D with no attached Timer, it will use this life time.
    /// </summary>
    [Export(PropertyHint.Range, "0, 1, 0.1, or_greater")]
    private float _defaultStepTime = 0.5f;
    private LinkedList<SequenceStep> _sequenceSteps = [];
    private LinkedListNode<SequenceStep> _currentStep;

    public override void _Ready()
    {
        foreach (Node node in _area3D.GetChildren())
        {
            if (node is not CollisionShape3D shape)
                continue;

            if (!RetrieveTimer(shape, out Timer timer))
            {
                timer = DefaultTimer();
                AddChild(timer);
            }

            AddSequenceShape(new SequenceStep(shape, timer));
        }

        _currentStep = _sequenceSteps.First;
    }

    private static bool RetrieveTimer(CollisionShape3D shape, out Timer timer)
    {
        foreach (Node node in shape.GetChildren())
            if (node is Timer t)
            {
                timer = t;
                return true;
            }

        timer = null;
        return false;
    }

    private Timer DefaultTimer() => new()
        {
            WaitTime = _defaultStepTime,
            ProcessCallback = Timer.TimerProcessCallback.Physics,
            OneShot = true,
        };

    private void AddSequenceShape(SequenceStep step)
    {
        step.Shape.Disabled = true;
        step.Timer.Timeout += StartNextStep;
        _sequenceSteps.AddLast(step);
    }

    public override void _ExitTree()
    {
        LinkedListNode<SequenceStep> step = _sequenceSteps.Last;

        while (step != null)
        {
            step.Value.Timer.Timeout -= StartNextStep;
            step = step.Previous;
            _sequenceSteps.RemoveLast();
        }
    }

    private void StartNextStep()
    {
        _currentStep.Value.Shape.Disabled = true;

        _currentStep = _currentStep.Next;
        StartStep();
    }

    private void StartStep()
    {
        if (_currentStep == null)
            return;

        _currentStep.Value.Shape.Disabled = false;
        _currentStep.Value.Timer.Start();
    }

    public override void StartSequence()
    {
        _currentStep = _sequenceSteps.First;
        StartStep();
    }

    public override void StopSequence()
    {
        _currentStep.Value.Shape.Disabled = true;
        _currentStep.Value.Timer.Stop();
    }
}