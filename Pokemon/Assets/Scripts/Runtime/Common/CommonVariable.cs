#region Packages

using UnityEngine;

#endregion

namespace Runtime.Common
{
    public static class CommonVariable
    {
        public static float PercentageOf(this float check, float max) => check / (max / 100);

        public static int RandomUniqueIndex(this int currentIndex, int listCount) =>
            currentIndex + (currentIndex + Random.Range(1, listCount - 1)) % listCount;

        public static float Clamp(this Vector2 bounds, float current) => Mathf.Clamp(current, bounds.x, bounds.y);

        public static float Clamp(ref this float current, float min, float max) => current = Mathf.Clamp(current, min, max);
    }
}