using Godot;

public static class MATH_Vector3Ext
{
    public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float deltaTime)
    {
        smoothTime = Mathf.Max(0.0001f, smoothTime); // Prevent division by zero

        float omega = 2.0f / smoothTime;
        float x = omega * deltaTime;
        float exp = 1.0f / (1.0f + x + 0.48f * x * x + 0.235f * x * x * x);

        Vector3 change = current - target;
        Vector3 temp = (currentVelocity + omega * change) * deltaTime;
        currentVelocity = (currentVelocity - omega * temp) * exp;

        return target + (change + temp) * exp;
    }

    public static Vector3 Flat(Vector3 vector) => new(vector.X, 0, vector.Z);
}