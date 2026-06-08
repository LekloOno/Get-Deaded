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

    public event Action<CrosshairShapeData>? NewLayerRequested;

    public override void _Ready()
    {
        Populate();
        
        ItemSelected += OnItemSelected;
    }

    private void Populate()
    {
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
        NewLayerRequested?.Invoke(From(type));
    }

    private CrosshairShapeData From(CrosshairLayerTypes type)
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
}
