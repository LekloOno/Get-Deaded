using Godot;

public static class KeyPressedArgExt
{
    public static bool IsStopped(this KeyPressedArgs arg) =>
        arg.WalkAxis.IsStopped();

    public static bool IsBacking(this KeyPressedArgs arg) =>
        arg.WalkAxis.IsBacking();

    public static bool IsStopped(this Vector2 axis) =>
        axis.X == 0 && axis.Y == 0;

    public static bool IsBacking(this Vector2 axis) =>
        axis.Y > 0;
}