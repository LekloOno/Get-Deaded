using System;
using Godot;

[Tool]
[GlobalClass]
public partial class CrosshairDataSyncer : Node
{
    private CrosshairRenderer _renderer = null!;

    public override void _EnterTree()
    {
        _renderer = GetParent<CrosshairRenderer>();
        if (_renderer == null)
        {
            GD.PushError($"{nameof(CrosshairDataSyncer)} must be a direct child of {nameof(CrosshairRenderer)}");
            return;
        }

        _renderer.DataSwapped += OnDataSwapped;

        ResyncAndRedraw();
    }

    public override void _ExitTree()
    {
        if (_renderer?.Data != null)
            Unsubscribe(_renderer.Data);
        if (_renderer != null)
            _renderer.DataSwapped -= OnDataSwapped;
    }

    private void OnDataSwapped(CrosshairData previous, CrosshairData next)
    {
        if (previous != null) Unsubscribe(previous);
        if (next     != null) Subscribe(next);
    }

    private void Subscribe(CrosshairData data)
    {
        data.PropertyChanged  += OnPropertyChanged;
        data.StructureChanged += OnStructureChanged;
        data.ShapeAdded       += OnShapeAdded;
        data.ShapeRemoved     += OnShapeRemoved;

        SubscribeOutline(data.OutlineData);
        SubscribeFill(data.FillData);

        foreach (var shape in data.Shapes)
        {
            // Editor guard
            if (shape == null) continue;
            SubscribeShape(shape);
        }
    }

    private void Unsubscribe(CrosshairData data)
    {
        data.PropertyChanged  -= OnPropertyChanged;
        data.StructureChanged -= OnStructureChanged;
        data.ShapeAdded       -= OnShapeAdded;
        data.ShapeRemoved     -= OnShapeRemoved;
        
        UnsubscribeOutline(data.OutlineData);
        UnsubscribeFill(data.FillData);
        
        foreach (var shape in data.Shapes)
        {
            // Editor guard
            if (shape == null) continue;
            UnsubscribeShape(shape);
        }
    }

    private void SubscribeShape(CrosshairShapeData shape)
    {
        shape.PropertyChanged += OnPropertyChanged;
        SubscribeOutline(shape.OutlineData);
        SubscribeFill(shape.FillData);
    }

    private void UnsubscribeShape(CrosshairShapeData shape)
    {
        shape.PropertyChanged -= OnPropertyChanged;
        UnsubscribeOutline(shape.OutlineData);
        UnsubscribeFill(shape.FillData);
    }

    private void SubscribeOutline(OutlineData outline)
    {
        if (outline != null)
            outline.PropertyChanged += OnPropertyChanged;
    }

    private void UnsubscribeOutline(OutlineData outline)
    {
        if (outline != null)
            outline.PropertyChanged -= OnPropertyChanged;
    }

    private void SubscribeFill(FillData fill)
    {
        if (fill != null)
            fill.PropertyChanged += OnPropertyChanged;
    }

    private void UnsubscribeFill(FillData fill)
    {
        if (fill != null)
            fill.PropertyChanged -= OnPropertyChanged;
    }

    private void OnShapeAdded(CrosshairShapeData shape)
    {
        SubscribeShape(shape);
        _renderer?.QueueRedraw();
    }

    private void OnShapeRemoved(CrosshairShapeData shape)
    {
        UnsubscribeShape(shape);
        _renderer?.QueueRedraw();
    }

    private void OnPropertyChanged() => _renderer?.QueueRedraw();

    private void OnStructureChanged() => ResyncAndRedraw();

    private void ResyncAndRedraw()
    {
        if (_renderer == null)
            return;
        if (_renderer.Data == null)
            return;

        Unsubscribe(_renderer.Data);
        Subscribe(_renderer.Data);

        _renderer.QueueRedraw();
    }
}