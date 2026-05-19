using Godot;

public static class UI_ScoreBoardExtensions
{
    public static string DisplayAccuracy(float? accuracy)
    {
        if (accuracy == null)
            return "N/A";
        else
            return Mathf.RoundToInt((float)accuracy * 100f) + " %";
    }
}