using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Runtime.Common
{
    public static class ExtMathf 
    {
        public static Vector3 LerpPosition(AnimationCurve curve,float time, Vector3 p0, Vector3 p1, Vector3 p2)
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
    }
}
