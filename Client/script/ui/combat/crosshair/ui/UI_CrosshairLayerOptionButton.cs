using System;
using Godot;

[GlobalClass]
public partial class UI_CrosshairLayerOptionButton : OptionButton
{
    public enum CrosshairLayerTypes
    {
        Dot,
        Cross,
        Circle,
        Square,
    }

    public event Action<CrosshairShapeData>? NewTypeRequested;

    private bool _populated = false;

    public override void _Ready()
    {
        Populate();
        ItemSelected += OnItemSelected;
    }

    public void Initialize(CrosshairShapeData shape)
    {
        Populate();
        SetType(From(shape));
    }

    private void Populate()
    {
        if (_populated)
            return;

        _populated = true;

        Clear();

        foreach (CrosshairLayerTypes type in Enum.GetValues<CrosshairLayerTypes>())
        {
            int index = ItemCount;

            AddItem($"{type.ToString().ToUpperInvariant()}");
            SetItemMetadata(index, (int)type);
        }
    }

    private void OnItemSelected(long index)
    {
        var type = (CrosshairLayerTypes)(int)GetItemMetadata((int)index);
        NewTypeRequested?.Invoke(From(type));
    }

    private void SetType(CrosshairLayerTypes type)
    {
        for (int i = 0; i < ItemCount; i++)
        {
            var item = (CrosshairLayerTypes)(int)GetItemMetadata(i);

            if (item == type)
            {
                Select(i);
                return;
            }
        }

        GD.PushWarning($"[UI_CrosshairLayerOptionButton] Type {type} is not available in this OptionButton.");
    }

    private static CrosshairShapeData From(CrosshairLayerTypes type)
    {
        return type switch
        {
            CrosshairLayerTypes.Dot     => new CrosshairDotData(),
            CrosshairLayerTypes.Cross   => new CrosshairCrossData(),
            CrosshairLayerTypes.Circle  => new CrosshairCircleData(),
            CrosshairLayerTypes.Square  => new CrosshairSquareData(),
            _ => new CrosshairDotData(),
        };
    }

    private static CrosshairLayerTypes From(CrosshairShapeData shape)
    {
        return shape switch
        {
            CrosshairDotData _ => CrosshairLayerTypes.Dot,
            CrosshairCrossData _ => CrosshairLayerTypes.Cross,
            CrosshairCircleData _ => CrosshairLayerTypes.Circle,
            CrosshairSquareData _ => CrosshairLayerTypes.Square,
            _ => CrosshairLayerTypes.Dot,
        };
    }
}
