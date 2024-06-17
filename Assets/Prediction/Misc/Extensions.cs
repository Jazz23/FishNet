using UnityEngine;

namespace Prediction
{
    public static class Extensions
    {
        public static void Deconstruct(this Vector2 vector, out float x, out float y)
        {
            x = vector.x;
            y = vector.y;
        }
    }
}