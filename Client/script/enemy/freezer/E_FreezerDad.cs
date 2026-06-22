using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

public partial class E_FreezerDad : Node3D, E_IEnemy
{
    [Export] private E_Freezer _base = null!;
    [Export] private Array<E_Freezer> _children = [];
    [Export] private DATA_FreezerDadSettings _settings = null!;
    [Export] private E_TargetAcquirer       _acquirer = null!;
    [Export] public  PCT_SimpleTraumaData   KillTraumaData  { get; private set; } = null!;
    public uint Score => _settings.Score;

    private readonly List<E_Freezer> _liveChildren = [];
    private readonly Stack<E_Freezer> _deadChildren = [];

    public GE_ICombatEntity? Target
    {
        get => _acquirer.Target;
        set {}
    }

    public PW_WeaponsHandler WeaponsHandler => null!;
    public GC_HealthManager HealthManager => null!;
    public GB_IExternalBodyManager Body => _base.Body;

    public event EnemyHealthEventHandler? Died;
    public event EnemyDisableEventHandler? Disabled;
    public event EnemyHealthEventHandler<DamageEventArgs>? Damaged;
    public event Action<E_IEnemy> Pooled;
    public event Action Spawned;

    public override void _Ready()
    {
        _base.Died += SpawnChildren;
        foreach (E_Freezer child in _children)
        {
            child.Pool();
            child.Died += CheckChildrenDeath;
            child.Disabled += ChildrenDisabled;
            _deadChildren.Push(child);
        }
    }

    private void SpawnChildren(E_IEnemy enemy, GC_Health senderLayer)
    {
        while (_deadChildren.TryPop(out E_Freezer? child))
        {
            child!.Spawn();
            child.Body.Teleport(Body.GlobalTransform.Origin + ComputeSpawnOffset());
            _liveChildren.Add(child);
        }
    }

    private Vector3 ComputeSpawnOffset()
    {
        GE_ICombatEntity? target = Target;

        if (target != null)
        {
            Vector3 toTarget = target.Body.GlobalTransform.Origin - Body.GlobalTransform.Origin;
            float distToTarget = toTarget.Length();

            if (distToTarget > 0.001f)
                return ComputeSphereIntersectionOffset(toTarget, distToTarget);
        }

        return ComputePlaneOffset(Body.GlobalTransform.Basis);
    }

    /// <summary>
    /// Picks a random point that is:
    ///   - at distance r from the parent (r sampled in [minR, maxR])
    ///   - at exactly distToTarget from the target
    ///
    /// The intersection of these two spheres is a circle.
    /// We pick a uniformly random point on that circle.
    /// </summary>
    private Vector3 ComputeSphereIntersectionOffset(Vector3 toTarget, float distToTarget)
    {
        // Area-uniform sample in the annulus: r is sqrt-distributed
        float r = SampleRadius();

        // If minRadius is impossibly large, we already dropped it by using
        // SampleRadius(). But we also need r <= 2*dist for the spheres to intersect.
        // Clamp down silently: the minimum radius constraint is dropped in this case.
        r = Mathf.Min(r, 2.0f * distToTarget - 0.001f);
        r = Mathf.Max(r, 0.001f);

        // The intersection circle lies at distance x along the parent-target axis.
        // Derived from: x**2 + ρ**2 = r**2, (dist - x)**2 + ρ**2 = dist**2
        // - x = r**2 / (2 * dist)
        float x = (r * r) / (2.0f * distToTarget);
        float rhoSq = r * r - x * x;
        float rho = rhoSq > 0f ? Mathf.Sqrt(rhoSq) : 0f;

        // Build an orthonormal frame around the parent-target axis
        Vector3 axisZ = toTarget / distToTarget;
        Vector3 axisX = axisZ.Cross(Vector3.Up);
        if (axisX.LengthSquared() < 0.001f)
            axisX = axisZ.Cross(Vector3.Right);
        axisX = axisX.Normalized();
        Vector3 axisY = axisZ.Cross(axisX);

        // Pick a random angle on the intersection circle
        float angle = GD.Randf() * Mathf.Tau;
        Vector3 radialDir = Mathf.Cos(angle) * axisX + Mathf.Sin(angle) * axisY;

        return axisZ * x + radialDir * rho;
    }

    /// <summary>
    /// Picks a random point in the parent's local XY plane
    /// </summary>
    private Vector3 ComputePlaneOffset(Basis basis)
    {
        float r = SampleRadius();
        float angle = GD.Randf() * Mathf.Tau;

        // Basis.X = local right, Basis.Y = local up.
        // Together they span the plane perpendicular to the facing direction.
        return basis.X * (r * Mathf.Cos(angle)) + basis.Y * (r * Mathf.Sin(angle));
    }

    /// <summary>
    /// Returns a radius sampled uniformly by area within [minR, maxR].
    /// sqrt(lerp(min**2, max**2, t)) gives area-uniform density in a disc/annulus.
    /// </summary>
    private float SampleRadius()
    {
        float t = GD.Randf();
        float minSq = _settings.ChildSpawnMinRadius * _settings.ChildSpawnMinRadius;
        float maxSq = _settings.ChildSpawnMaxRadius * _settings.ChildSpawnMaxRadius;
        return Mathf.Sqrt(Mathf.Lerp(minSq, maxSq, t));
    }

    private void CheckChildrenDeath(E_IEnemy enemy, GC_Health senderLayer)
    {
        if (enemy is not E_Freezer child)
            return;

        if (!_liveChildren.Remove(child))
            return;

        _deadChildren.Push(child);

        if (_liveChildren.Count == 0)
            Died?.Invoke(this, null!);
    }

    private void ChildrenDisabled(E_IEnemy enemy)
    {
        enemy.Pool();
        if (_liveChildren.Count == 0)
            Disabled?.Invoke(this);
    }

    public void Pool()
    {
        _base.Pool();

        foreach (E_Freezer child in _liveChildren)
        {
            child.Pool();
            _deadChildren.Push(child);
        }

        _liveChildren.Clear();
    }

    public void Spawn()
    {
        _base.Spawn();
    }
}