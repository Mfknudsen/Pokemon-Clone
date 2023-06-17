using UnityEngine;

namespace Runtime.Common
{
    public static class ExtMathf
    {
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
        public static bool PointWithin2DTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c)
        {
            float w1 = (a.x * (c.y - a.y) + (point.y - a.y) * (c.x - a.x) - point.x * (c.y - a.y)) /
                       ((b.y - a.y) * (c.x - a.x) - (b.x - a.x) * (c.y - a.y));

            float w2 = (point.y - a.y - w1 * (b.y - a.y)) /
                       (c.y - a.y);

            return w1 >= 0f && w2 >= 0 && w1 + w2 <= 1f;
        }
    }
}
