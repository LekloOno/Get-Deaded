using System.Collections.Generic;
using Godot;

/// <summary>
/// A PhysicalBoneSimulator3D wrapper. <br/>
/// Generates associated PHX_RagdollBone.
/// </summary>
[GlobalClass]
public partial class PHX_ActiveRagdollSim : Node
{
    [Export] private PhysicalBoneSimulator3D _simulator;
    [Export] private Skeleton3D _animSkeleton;
    [Export] private Skeleton3D _ragdollSkeleton;
    [Export] private float blend = 1f;

    private struct BoneData
    {
        public int BoneIdx;
        public int ParentIdx;
        public PHX_ActiveRagdollBone Bone;
        public float Blend; // 0 = floppy, 1 = animated
    }

    private List<BoneData> _bones = [];

    private void CacheBones()
    {
        foreach (Node child in _simulator.GetChildren())
        {
            if (child is not PhysicalBone3D pb)
                continue;

            PHX_ActiveRagdollBone ragdollBone = new(pb, this, 110f, 150f, 10f, 20f);
            InitHurtBox(pb, ragdollBone);

            int boneIdx = pb.GetBoneId();

            _bones.Add(new BoneData
            {
                BoneIdx = boneIdx,
                ParentIdx = _ragdollSkeleton.GetBoneParent(boneIdx),
                Bone = ragdollBone,
                Blend = 1f
            });
        }
    }

    private void InitHurtBox(PhysicalBone3D pb, PHX_ActiveRagdollBone ragdollBone)
    {
        foreach (Node child in pb.GetChildren())
            if (child is GC_HurtBox hurtBox)
                hurtBox.InitRagdollBone(ragdollBone);
    }

    public override void _Ready()
    {
        if (_ragdollSkeleton == null || _simulator == null)
            SetPhysicsProcess(false);
        else
        {
            CacheBones();
            StartSimulation();
        }
    }

    public void StartSimulation() => _simulator.PhysicalBonesStartSimulation();
    public void StopSimulation() => _simulator.PhysicalBonesStopSimulation();
    public void Hit(Vector3 impulse, Vector3 ?from)
    {
        StartSimulation();
        foreach (BoneData bone in _bones)
            bone.Bone.ApplyImpulse(impulse, from);
    }

    public override void _PhysicsProcess(double delta)
    {
        for (int i = 0; i < _bones.Count; i++)
        {
            var data = _bones[i];

            Transform3D targetTransform = _ragdollSkeleton.GlobalTransform * _ragdollSkeleton.GetBoneGlobalPose(data.BoneIdx);
            Transform3D currentTransform = data.Bone.GlobalTransform;

            data.Bone.ActiveRagdoll(targetTransform, currentTransform, delta, blend);
        }
    }
}