using System.Numerics;
using System;

namespace CalamityTweaks.Helpers
{
    public static class Funcs
    {
        public static bool InRange(int val, int min, int max)
        {
            return val >= min && val <= max;
        }

        public static Vector2 toPolar(Vector2 cartesianCoords)
        {
            return new(cartesianCoords.Length(), MathF.Atan2(cartesianCoords.Y, cartesianCoords.X));
        }

        public static Vector2 toCartesian(Vector2 polarCoords)
        {
            return new(polarCoords.X * MathF.Cos(polarCoords.Y), polarCoords.X * MathF.Sin(polarCoords.Y));
        }
    }
}