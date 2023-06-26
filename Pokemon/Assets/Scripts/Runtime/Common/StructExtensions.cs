#region Libraries

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Runtime.Common
{
    public static class StructExtensions
    {
        #region int

        public static int RandomUniqueIndex(this int currentIndex, int listCount) =>
            currentIndex + (currentIndex + Random.Range(1, listCount - 1)) % listCount;

        #endregion

        #region float

        public static float PercentageOf(this float check, float max) => check / (max / 100);

        public static float Clamp(this Vector2 bounds, float current) => Mathf.Clamp(current, bounds.x, bounds.y);

        public static float Clamp(this float current, float min, float max) => Mathf.Clamp(current, min, max);

        public static void RefClamp(this ref float current, float min, float max) =>
            current = Mathf.Clamp(current, min, max);

        public static float Squared(this float current) => current * current;

        #endregion

        #region Vector2

        public static System.Numerics.Vector2 ToNurmerics(this Vector2 target) => new(target.x, target.y);

        public static Vector3 ToV3(this Vector2 t, float y) => new(t.x, y, t.y);

        #endregion

        #region Vector3

        public static Vector2 XZ(this Vector3 target) => new(target.x, target.z);

        public static float QuickSquareDistance(this Vector3 point1, Vector3 point2) => (point1 - point2).sqrMagnitude;

        public static bool QuickDistanceLessThen(this Vector3 point1, Vector3 point2, float distance) =>
            QuickSquareDistance(point1, point2) < distance * distance;

        public static float ShortDistancePointToLine(this Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 line = lineEnd - lineStart;
            Vector3 startToPoint = point - lineStart;

            float area = Vector3.Cross(line, startToPoint).magnitude;

            return area / line.magnitude;
        }

        #endregion

        #region Quaternion

        public static Vector3 ForwardFromRotation(this Quaternion quaternion) => quaternion * Vector3.forward;

        public static Vector3 UpFromRotation(this Quaternion quaternion) => quaternion * Vector3.up;

        public static Vector3 RightFromRotation(this Quaternion quaternion) => quaternion * Vector3.right;

        #endregion

        #region bool

        public static void Reverse(this ref bool b) => b = !b;

        #endregion

        #region Array/List

        public static bool ContainsAny<T>(this T[] target, T[] other)
        {
            foreach (T item in target)
            {
                foreach (T otherItem in other)
                {
                    if (item.Equals(otherItem))
                        return true;
                }
            }

            return false;
        }

        public static T RandomFrom<T>(this T[] target) =>
            target[Random.Range(0, target.Length)];

        public static T RandomFrom<T>(this List<T> target) =>
            target[Random.Range(0, target.Count)];

        public static T[] SharedBetween<T>(this T[] target, T[] other)
        {
            List<T> result = new();

            foreach (T a in target)
            {
                foreach (T b in other)
                {
                    if (a.Equals(b))
                    {
                        result.Add(a);
                        break;
                    }
                }
            }

            return result.ToArray();
        }

        #endregion
    }
}