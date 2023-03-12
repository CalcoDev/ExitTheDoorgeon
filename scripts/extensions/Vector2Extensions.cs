using Godot;
using Utils;

namespace Game.Extensions;

public static class Vector2Extensions
{
    public static string Vector2ToDirString(this Vector2 vector, float epsilon = 0.02f)
    {
        var d = vector.Dot(Vector2.Right);
        if (Calc.FloatEquals(d, 1f, epsilon))
            return "right";
        if (Calc.FloatEquals(d, -1f, epsilon))
            return "left";

        d = vector.Dot(Vector2.Up);
        if (Calc.FloatEquals(d, 1f, epsilon))
            return "up";
        if (Calc.FloatEquals(d, -1f, epsilon))
            return "down";

        if (vector.X > 0)
            return "right";
        if (vector.X < 0)
            return "left";

        if (vector.Y > 0)
            return "up";
        if (vector.Y < 0)
            return "down";

        return "none";
    }
}