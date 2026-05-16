using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.Text.Json;

public static class InputEventSerder
{
    public static bool TryToInputEventList(this string inputListString, out List<InputEvent> inputEvents)
    {
        inputEvents = new List<InputEvent>();
        if (string.IsNullOrWhiteSpace(inputListString))
            return false;

        var stringList = JsonSerializer.Deserialize<List<string>>(inputListString);

        if (stringList == null)
            return false;

        foreach (var s in stringList)
            if (s.TryToInputEvent(out var inputEvent))
                inputEvents.Add(inputEvent);

        return inputEvents.Count > 0;
    }

    public static bool TryToInputEvent(this string inputString, out InputEvent inputEvent)
    {
        inputEvent = null;

        if (string.IsNullOrWhiteSpace(inputString))
            return false;


        if (inputString.StartsWith("mouse_"))
        {
            var indexStr = inputString.Substring("mouse_".Length);

            if (int.TryParse(indexStr, out int buttonIndex))
            {
                inputEvent = new InputEventMouseButton()
                {
                    ButtonIndex = (MouseButton)buttonIndex
                };

                return true;
            }
        }
        else
        { 
            if (int.TryParse(inputString, out int keyCode))
            {
                inputEvent = new InputEventKey
                {
                    PhysicalKeycode = (Key)keyCode
                };

                return true;
            }
        }

        return false;
    }

    public static bool TryToStringList(this List<InputEvent> inputEvents, out string serialized)
    {
        serialized = string.Empty;

        if (inputEvents == null)
            return false;

        var list = new List<string>();

        foreach (var inputEvent in inputEvents)
            if (inputEvent.TryToString(out var s))
                list.Add(s);

        serialized = JsonSerializer.Serialize(list);
        return list.Count > 0;
    }

    public static bool TryToStringList(this Array<EditableInputEvent> inputEvents, out string serialized)
    {
        serialized = string.Empty;

        if (inputEvents == null)
            return false;

        var list = new List<string>();

        foreach (var inputEvent in inputEvents)
            if (inputEvent.InputEvent.TryToString(out var s))
                list.Add(s);

        serialized = JsonSerializer.Serialize(list);
        return list.Count > 0;
    }


    public static bool TryToString(this InputEvent inputEvent, out string serialized)
    {
        serialized = string.Empty;

        if (inputEvent == null)
            return false;

        if (inputEvent is InputEventMouseButton mouseEvent)
        {
            serialized = "mouse_" + ((int)mouseEvent.ButtonIndex).ToString();
            return true;
        }

        if (inputEvent is InputEventKey keyEvent)
        {
            serialized = ((int)keyEvent.PhysicalKeycode).ToString();
            return true;
        }

        return false;
    }
}