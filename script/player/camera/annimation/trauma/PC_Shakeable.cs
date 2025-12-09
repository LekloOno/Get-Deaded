using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class PC_Shakeable : Area3D
{
    [Export] private float _reductionRate = 1f;
    [Export] private FastNoiseLite _noise;
    [Export] private float _noiseSpeed = 50f;
    [Export] private PC_TraumaLayer _baseLayer;
    [Export] private Vector3 _maxRotation = new(10f, 10f, 5f);

    private List<PC_TraumaLayer> _traumaLayers = [];

    private float _trauma = 0f;
    private float _time = 0f;

    public override void _Ready()
    {
        _traumaLayers.Add(_baseLayer);
        CollisionLayer = CONF_Collision.Layers.Trauma;
        CollisionMask = 0;
    }

    public override void _Process(double delta)
    {
        _time += (float)delta;

        Vector3 shakeAngleIntensity = Vector3.Zero;
        foreach(PC_TraumaLayer layer in _traumaLayers)
            shakeAngleIntensity += layer.GetShakeAngleIntensity((float) delta, _time);

        RotationDegrees = _maxRotation * shakeAngleIntensity;
        Transform = Transform.Orthonormalized();
    }

    public void AddTrauma(float amount) => _baseLayer.AddTrauma(amount);
    public void AddClampedTrauma(float amount, float max) => _baseLayer.AddClampedTrauma(amount, max);
    public void AddLayer(PC_TraumaLayer layer) => _traumaLayers.Add(layer);
    public void RemoveLayer(PC_TraumaLayer layer) => _traumaLayers.Remove(layer);
}