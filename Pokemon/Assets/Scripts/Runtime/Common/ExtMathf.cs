#region Libraries

using System;
using UnityEngine;

#endregion

namespace Runtime.Common
{
    public static class ExtMathf
    {
        public static bool LineIntersect2D(Vector2 startA, Vector2 endA, Vector2 startB, Vector2 endB)
        {
            Vector2 dirA = endA - startA, dirB = endB - startB;

            float dot = Vector2.Dot(dirA, dirB);

            if (dot == 0)
                return false;

            Vector2 c = dirB - dirA;
            float t = Vector2.Dot(c, dirB) / dot;
            if (t < 0 || t > 1)
                return false;

            t = Vector2.Dot(c, dirA) / dot;
            return !(t < 0 || t > 1);
        }

        public static Vector3 LerpPosition(AnimationCurve curve, float time, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float curveTime = curve.Evaluate(time);

            float u = 1 - curveTime;
            float tSquared = curveTime * curveTime;
            float uSquared = u * u;
            Vector3 result = uSquared * p0;
            result += 2 * u * curveTime * p1;
            result += tSquared * p2;

            return result;
        }

        //https://www.youtube.com/watch?v=HYAgJN3x4GA
        public static bool PointWithinTriangle2D(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
        {
            float w1 = (a.x * (c.y - a.y) + (point.y - a.y) * (c.x - a.x) - point.x * (c.y - a.y)) /
                       ((b.y - a.y) * (c.x - a.x) - (b.x - a.x) * (c.y - a.y));

            float w2 = (point.y - a.y - w1 * (b.y - a.y)) /
                       (c.y - a.y);

            return w1 >= 0f && w2 >= 0 && w1 + w2 <= 1f;
        }

        public static bool LineIntersectTriangle(Vector3 p, float pUpDist, float pLowDist, Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 ab = b - a, ac = c - a;
            Vector3 normal = Vector3.Cross(ab, ac);

            Vector3 p1 = p + normal * pLowDist, p2 = p + normal * pUpDist;

            float det = Vector3.Dot(p2 - p1, normal);
            float invdet = 1f / det;

            Vector3 A0 = p1 - a;
            Vector3 DA0 = Vector3.Cross(A0, p2 - p1);
            float u = Vector3.Dot(ac, DA0) * invdet;
            float v = -Vector3.Dot(ab, DA0) * invdet;
            float t = Vector3.Dot(A0, normal) * invdet;

            return det >= 1e-6 && t >= 0f && u >= 0f && v >= 0f && u + v >= 1f;
        }

        public static bool TriangleIntersectTriangle(System.Numerics.Vector3 a1, System.Numerics.Vector3 a2, System.Numerics.Vector3 a3, System.Numerics.Vector3 b1, System.Numerics.Vector3 b2, System.Numerics.Vector3 b3)
        {
            float aMinX = a1.X < a2.X && a1.X < a3.X ? a1.X : a2.X < a3.X ? a2.X : a3.X,
                aMaxX = a1.X > a2.X && a1.X > a3.X ? a1.X : a2.X > a3.X ? a2.X : a3.X,
                aMinY = a1.Y < a2.Y && a1.Y < a3.Y ? a1.Y : a2.Y < a3.Y ? a2.Y : a3.Y,
                aMaxY = a1.Y > a2.Y && a1.Y > a3.Y ? a1.Y : a2.Y > a3.Y ? a2.Y : a3.Y,
                aMinZ = a1.Z < a2.Z && a1.Z < a3.Z ? a1.Z : a2.Z < a3.Z ? a2.Z : a3.Z,
                aMaxZ = a1.Z > a2.Z && a1.Z > a3.Z ? a1.Z : a2.Z > a3.Z ? a2.Z : a3.Z;

            float bMinX = b1.X < b2.X && b1.X < b3.X ? b1.X : b2.X < b3.X ? b2.X : b3.X,
                bMaxX = b1.X > b2.X && b1.X > b3.X ? b1.X : b2.X > b3.X ? a2.X : b3.X,
                bMinY = b1.Y < b2.Y && b1.Y < b3.Y ? b1.Y : b2.Y < b3.Y ? a2.Y : b3.Y,
                bMaxY = b1.Y > b2.Y && b1.Y > b3.Y ? b1.Y : b2.Y > b3.Y ? a2.Y : b3.Y,
                bMinZ = b1.Z < b2.Z && b1.Z < b3.Z ? b1.Z : b2.Z < b3.Z ? a2.Z : b3.Z,
                bMaxZ = b1.Z > b2.Z && b1.Z > b3.Z ? b1.Z : b2.Z > b3.Z ? a2.Z : b3.Z;

            if (aMaxX < bMinX || aMinX > bMaxX ||
                aMaxY < bMinY || aMinY > bMaxY ||
                aMaxZ < bMinZ || aMinZ > bMaxZ)
                return false;

            if (!CheckColisionLookAt(a1, a2, a3, b1, b2, b3))
                return false;
            if (!CheckColisionLookAt(a2, a3, a1, b1, b2, b3))
                return false;
            if (!CheckColisionLookAt(a3, a1, a2, b1, b2, b3))
                return false;

            if (!CheckColisionLookAt(b1, b2, b3, a1, a2, a3))
                return false;
            if (!CheckColisionLookAt(b2, b3, b1, a1, a2, a3))
                return false;
            if (!CheckColisionLookAt(b3, b1, b2, a1, a2, a3))
                return false;

            return CheckColisionAllOnOneSide(a1, a2, a3, b1, b2, b3);
        }

        private static bool CheckColisionAllOnOneSide(System.Numerics.Vector3 t1a, System.Numerics.Vector3 t1b, System.Numerics.Vector3 t1c, System.Numerics.Vector3 t2a, System.Numerics.Vector3 t2b, System.Numerics.Vector3 t2c)
        {
            //simply performs a transformation to check if all points on one triangle are on the same side of the other triangle
            System.Numerics.Matrix4x4 m = System.Numerics.Matrix4x4.CreateLookAt(t1a, t1b, t1c - t1a);
            t2a = System.Numerics.Vector3.Transform(t2a, m);
            t2b = System.Numerics.Vector3.Transform(t2b, m);
            t2c = System.Numerics.Vector3.Transform(t2c, m);

            if (t2a.X < 0 && t2b.X < 0 && t2c.X < 0)
                return false;

            return !(0 < t2a.X && 0 < t2b.X && 0 < t2c.X);
        }

        private static bool CheckColisionLookAt(System.Numerics.Vector3 t1a, System.Numerics.Vector3 t1b, System.Numerics.Vector3 t1c, System.Numerics.Vector3 t2a, System.Numerics.Vector3 t2b, System.Numerics.Vector3 t2c)
        {
            //performs a transformation and checks if all points of the one triangle are under the other triangle after the transformation
            System.Numerics.Matrix4x4 m = System.Numerics.Matrix4x4.CreateLookAt(t1a, t1b, t1c - t1a);
            t1a = System.Numerics.Vector3.Transform(t1a, m);
            if (0 < Math.Abs(t1a.X) || 0 < Math.Abs(t1a.Y) || 0 < Math.Abs(t1a.Z))
                return false;

            t1b = System.Numerics.Vector3.Transform(t1b, m);
            if (0 < Math.Abs(t1a.X) || 0 < Math.Abs(t1a.Y))
                return false;
            t1c = System.Numerics.Vector3.Transform(t1c, m);
            if (0 < Math.Abs(t1a.X))
                return false;

            t2a = System.Numerics.Vector3.Transform(t2a, m);
            t2b = System.Numerics.Vector3.Transform(t2b, m);
            t2c = System.Numerics.Vector3.Transform(t2c, m);
            if (t2a.Y < 0 && t2b.Y < 0 && t2c.Y < 0)
                return false;
            return true;
        }
    }
}
