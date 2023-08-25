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
        public static bool LineIntersect2D(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
        {
            //Line1
            float A1 = end1.y - start1.y;
            float B1 = start1.x - end1.x;
            float C1 = A1 * start1.x + B1 * start1.y;

            //Line2
            float A2 = end2.y - start2.y;
            float B2 = start2.x - end2.x;
            float C2 = A2 * start2.x + B2 * start2.y;

            float denominator = A1 * B2 - A2 * B1;

            if (denominator == 0)
                return false;

            Vector2 point = new Vector2((B2 * C1 - B1 * C2) / denominator, (A1 * C2 - A2 * C1) / denominator);

            if (point == start1 || point == end1 ||
                point == start2 || point == end2)
                return false;

            const float tolerance = .001f;
            if ((point.x > MathF.Min(start1.x, end1.x) + tolerance &&
                 point.x < MathF.Max(start1.x, end1.x) - tolerance) &&
                (point.x > MathF.Min(start2.x, end2.x) + tolerance &&
                 point.x < MathF.Max(start2.x, end2.x) - tolerance) &&
                (point.y > MathF.Min(start1.y, end1.y) + tolerance &&
                 point.y < MathF.Max(start1.y, end1.y) - tolerance) &&
                (point.y > MathF.Min(start2.y, end2.y) + tolerance &&
                 point.y < MathF.Max(start2.y, end2.y) - tolerance))
            {
                Debug.DrawLine(start1.ToV3(4), end1.ToV3(4), Color.red);
                Debug.DrawLine(start2.ToV3(4), end2.ToV3(4), Color.green);

                Debug.DrawRay(point.ToV3(4), Vector3.up, Color.yellow);

                return true;
            }

            return false;
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
        public static bool PointWithinTriangle2D(Vector2 point, Vector2 a, Vector2 b, Vector2 c,
            float tolerance = .001f)
        {
            float w1 = (a.x * (c.y - a.y) + (point.y - a.y) * (c.x - a.x) - point.x * (c.y - a.y)) /
                       ((b.y - a.y) * (c.x - a.x) - (b.x - a.x) * (c.y - a.y));

            float w2 = (point.y - a.y - w1 * (b.y - a.y)) /
                       (c.y - a.y);

            return w1 >= tolerance && w2 >= tolerance && w1 + w2 <= 1f - tolerance;
        }

        public static bool TriangleIntersect2D(Vector2 a1, Vector2 a2, Vector2 a3, Vector2 b1, Vector2 b2, Vector2 b3)
        {
            return (LineIntersect2D(a1, a2, b1, b2) ||
                    LineIntersect2D(a1, a3, b1, b2) ||
                    LineIntersect2D(a2, a3, b1, b2) ||
                    LineIntersect2D(a1, a2, b1, b3) ||
                    LineIntersect2D(a1, a3, b1, b3) ||
                    LineIntersect2D(a2, a3, b1, b3) ||
                    LineIntersect2D(a1, a2, b2, b3) ||
                    LineIntersect2D(a1, a3, b2, b3) ||
                    LineIntersect2D(a2, a3, b2, b3));
        }

        public static Vector2 ClosetPointOnLine(Vector2 point, Vector2 start, Vector2 end)
        {
            //Get heading
            Vector2 heading = (end - start);
            float magnitudeMax = heading.magnitude;
            heading.Normalize();

            //Do projection from the point but clamp it
            Vector2 lhs = point - start;
            float dotP = Vector2.Dot(lhs, heading);
            dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);

            return start + heading * dotP;
        }

        public static float ClosesPointValue(Vector2 point, Vector2 start, Vector2 end)
        {
            //Get heading
            Vector2 heading = (end - start);
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
            y = y * (threehalfs - (x2 * y * y));

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