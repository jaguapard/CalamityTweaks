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

        public static Vector2 toPolar(Vector2 pos)
        {
            return new(pos.Length(), MathF.Atan2(pos.Y, pos.X));
        }
    }
}