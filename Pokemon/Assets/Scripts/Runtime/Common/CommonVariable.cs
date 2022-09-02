using UnityEngine;

namespace Runtime.Common
{
    public static class CommonVariable
    {
        public static float PercentageOf(this float check, float max)
        {
            return check / (max / 100);
        }

        public static int RandomUniqueIndex(this int currentIndex, int listCount)
        {
            return currentIndex + (currentIndex + Random.Range(1, listCount - 1)) % listCount;
        }
    }
}