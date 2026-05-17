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
    [Export] private Skeleton3D _skeleton;

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

            PHX_ActiveRagdollBone ragdollBone = new(pb, this);
            InitHurtBox(pb, ragdollBone);

            int boneIdx = pb.GetBoneId();

            _bones.Add(new BoneData
            {
                BoneIdx = boneIdx,
                ParentIdx = _skeleton.GetBoneParent(boneIdx),
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
        CacheBones();
    }

    public void StartSimulation() => _simulator.PhysicalBonesStartSimulation();
    public void StopSimulation() => _simulator.PhysicalBonesStopSimulation();
    public void Hit(Vector3 impulse, Vector3 ?from)
    {
        StartSimulation();
        foreach (BoneData bone in _bones)
            bone.Bone.ApplyImpulse(impulse, from);
    }
}