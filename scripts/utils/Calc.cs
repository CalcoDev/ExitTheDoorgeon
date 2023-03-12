using Godot;

namespace Utils;

public static class Calc
{
    public static bool SameSign(float a, float b)
    {
        return (a < 0) == (b < 0);
    }

    public static bool SameSignZero(float a, float b)
    {
        if (a == 0 || b == 0)
            return true;

        return (a < 0) == (b < 0);
    }

    public static float ApproachF(float current, float target, float maxDelta)
    {
        return current < target ? Mathf.Min(current + maxDelta, target) : Mathf.Max(current - maxDelta, target);
    }

    public static Vector2 ApproachV(Vector2 current, Vector2 target, float maxDelta)
    {
        var c = new Vector2();
        c.X = ApproachF(current.X, target.X, maxDelta);
        c.Y = ApproachF(current.Y, target.Y, maxDelta);

        return c;
    }

    public static bool FloatEquals(float a, float b, float epsilon = 0.001f)
    {
        return Mathf.Abs(a - b) < epsilon;
    }
}