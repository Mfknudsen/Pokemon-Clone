#region Libraries

using System;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

#endregion

namespace Runtime.Core
{
    public static class MathC
    {
        public static bool LineIntersect2DWithTolerance(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
        {
            //Line1
            float a1 = end1.y - start1.y;
            float b1 = start1.x - end1.x;
            float c1 = a1 * start1.x + b1 * start1.y;

            //Line2
            float a2 = end2.y - start2.y;
            float b2 = start2.x - end2.x;
            float c2 = a2 * start2.x + b2 * start2.y;

            float denominator = a1 * b2 - a2 * b1;

            if (denominator == 0)
                return false;

            Vector2 point = new Vector2((b2 * c1 - b1 * c2) / denominator, (a1 * c2 - a2 * c1) / denominator);

            if (point == start1 || point == end1 ||
                point == start2 || point == end2)
                return false;

            const float tolerance = .001f;

            return point.x > MathF.Min(start1.x, end1.x) + tolerance &&
                   point.x < MathF.Max(start1.x, end1.x) - tolerance &&
                   point.x > MathF.Min(start2.x, end2.x) + tolerance &&
                   point.x < MathF.Max(start2.x, end2.x) - tolerance &&
                   point.y > MathF.Min(start1.y, end1.y) + tolerance &&
                   point.y < MathF.Max(start1.y, end1.y) - tolerance &&
                   point.y > MathF.Min(start2.y, end2.y) + tolerance &&
                   point.y < MathF.Max(start2.y, end2.y) - tolerance;
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

        public static bool PointWithinTriangle2DWithTolerance(Vector2 point, Vector2 a, Vector2 b, Vector2 c,
            float tolerance = .001f)
        {
            float s1 = c.y - a.y + 0.0001f;
            float s2 = c.x - a.x;
            float s3 = b.y - a.y;
            float s4 = point.y - a.y;

            float w1 = (a.x * s1 + s4 * s2 - point.x * s1) / (s3 * s2 - (b.x - a.x + 0.0001f) * s1);
            float w2 = (s4 - w1 * s3) / s1;
            return w1 >= tolerance && w2 >= tolerance && w1 + w2 <= 1f - tolerance;
        }

        //https://www.youtube.com/watch?v=HYAgJN3x4GA
        public static bool PointWithinTriangle2D(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
        {
            float s1 = c.y - a.y + 0.0001f;
            float s2 = c.x - a.x;
            float s3 = b.y - a.y;
            float s4 = point.y - a.y;

            float w1 = (a.x * s1 + s4 * s2 - point.x * s1) / (s3 * s2 - (b.x - a.x + 0.0001f) * s1);
            float w2 = (s4 - w1 * s3) / s1;
            return w1 >= 0 && w2 >= 0 && w1 + w2 <= 1;
        }

        //https://www.youtube.com/watch?v=HYAgJN3x4GA
        public static bool PointWithinTriangle2D(Vector2 point, Vector2 a, Vector2 b, Vector2 c, out float w1,
            out float w2)
        {
            float s1 = c.y - a.y + 0.0001f;
            float s2 = c.x - a.x;
            float s3 = b.y - a.y;
            float s4 = point.y - a.y;

            w1 = (a.x * s1 + s4 * s2 - point.x * s1) / (s3 * s2 - (b.x - a.x + 0.0001f) * s1);
            w2 = (s4 - w1 * s3) / s1;
            return w1 >= 0 && w2 >= 0 && w1 + w2 <= 1;
        }

        public static bool TriangleIntersect2D(Vector2 a1, Vector2 a2, Vector2 a3, Vector2 b1, Vector2 b2, Vector2 b3)
        {
            return LineIntersect2DWithTolerance(a1, a2, b1, b2) ||
                   LineIntersect2DWithTolerance(a1, a3, b1, b2) ||
                   LineIntersect2DWithTolerance(a2, a3, b1, b2) ||
                   LineIntersect2DWithTolerance(a1, a2, b1, b3) ||
                   LineIntersect2DWithTolerance(a1, a3, b1, b3) ||
                   LineIntersect2DWithTolerance(a2, a3, b1, b3) ||
                   LineIntersect2DWithTolerance(a1, a2, b2, b3) ||
                   LineIntersect2DWithTolerance(a1, a3, b2, b3) ||
                   LineIntersect2DWithTolerance(a2, a3, b2, b3);
        }

        public static Vector2 ClosetPointOnLine(Vector2 point, Vector2 start, Vector2 end)
        {
            //Get heading
            Vector2 heading = end - start;
            float magnitudeMax = heading.magnitude;
            heading.Normalize();

            //Do projection from the point but clamp it
            Vector2 lhs = point - start;
            float dotP = Vector2.Dot(lhs, heading);
            dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);

            return start + heading * dotP;
        }

        public static Vector3 ClosetPointOnLine(Vector3 point, Vector3 start, Vector3 end)
        {
            //Get heading
            Vector3 heading = end - start;
            float magnitudeMax = heading.magnitude;
            heading.Normalize();

            //Do projection from the point but clamp it
            Vector3 lhs = point - start;
            float dotP = Vector3.Dot(lhs, heading);
            dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);

            return start + heading * dotP;
        }

        public static float ClosestPointValue(Vector2 point, Vector2 start, Vector2 end)
        {
            //Get heading
            Vector2 heading = end - start;
            float magnitudeMax = heading.magnitude;
            heading.Normalize();

            //Do projection from the point but clamp it
            Vector2 lhs = point - start;
            float dotP = Vector2.Dot(lhs, heading);
            return Mathf.Clamp(dotP, 0f, magnitudeMax);
        }

        public static Vector3 FastSqrt(Vector3 target) =>
            new Vector3(FastSqrt(target.x), FastSqrt(target.y), FastSqrt(target.z));

        public static Vector2 FastSqrt(Vector2 target) =>
            new Vector2(FastSqrt(target.x), FastSqrt(target.y));

        public static float FastInverseSqrt(float number)
        {
            // ReSharper disable once IdentifierTypo
            const float threehalfs = 1.5f;
            float x2 = number * .5f;
            float y = number;
            uint i = BitConverter.ToUInt32(BitConverter.GetBytes(y), 0);
            i = 0x5f3759df - (i >> 1);
            y = BitConverter.ToSingle(BitConverter.GetBytes(i), 0);
            y = y * (threehalfs - x2 * y * y);

            return y;
        }

        public static float FastSqrt(float number)
        {
            if (number < 2)
                return number;

            //Repeat for better approximation
            float a = 1000;
            float b = number / a;
            a = (a + b) * .5f;

            b = number / a;
            a = (a + b) * .5f;

            b = number / a;
            a = (a + b) * .5f;

            b = number / a;
            a = (a + b) * .5f;

            b = number / a;
            a = (a + b) * .5f;

            return a;
        }

        public static bool IsPointLeftToVector(Vector2 lineA, Vector2 lineB, Vector2 point)
            => (lineB.x - lineA.x) * (point.y - lineA.y) -
                (lineB.y - lineA.y) * (point.x - lineA.x) > 0;

        public static float QuickCircleIntersectCircleArea(Vector3 center1, Vector3 center2, float radius1,
            float radius2, float height1, float height2)
        {
            if (center1.y > center2.y + height2 || center2.y > center1.y + height1)
                return 0;

            float squaredRadius1 = radius1.Squared(),
                squaredRadius2 = radius2.Squared();

            float c = FastSqrt((center2.x - center1.x) * (center2.x - center1.x) +
                               (center2.z - center1.z) * (center2.z - center1.z));

            float phi = Mathf.Acos((squaredRadius1 + c * c - squaredRadius2) / (2 * radius1 * c)) * 2;
            float theta = Mathf.Acos((squaredRadius2 + c * c - squaredRadius1) / (2 * radius2 * c)) * 2;

            float area1 = 0.5f * theta * squaredRadius2 - 0.5f * squaredRadius2 * Mathf.Sin(theta);
            float area2 = 0.5f * phi * squaredRadius1 - 0.5f * squaredRadius1 * Mathf.Sin(phi);

            return (area1 + area2) * Mathf.Abs(height1 - height2);
        }
    }
}