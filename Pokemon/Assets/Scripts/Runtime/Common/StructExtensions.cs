#region Packages

using UnityEngine;

#endregion

namespace Runtime.Common
{
    public static class StructExtensions
    {
        public static float PercentageOf(this float check, float max) => check / (max / 100);

        public static int RandomUniqueIndex(this int currentIndex, int listCount) =>
            currentIndex + (currentIndex + Random.Range(1, listCount - 1)) % listCount;

        public static float Clamp(this Vector2 bounds, float current) => Mathf.Clamp(current, bounds.x, bounds.y);

        public static float Clamp(this float current, float min, float max) => Mathf.Clamp(current, min, max);

        public static void RefClamp(this ref float current, float min, float max) => current = Mathf.Clamp(current, min, max);

        public static Vector3 Forward(this Quaternion quaternion) => quaternion * Vector3.forward;
        public static Vector3 Up(this Quaternion quaternion) => quaternion * Vector3.up;
        public static Vector3 Right(this Quaternion quaternion) => quaternion * Vector3.right;
    }
}