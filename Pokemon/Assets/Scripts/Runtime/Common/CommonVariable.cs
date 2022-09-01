#region Packages

#endregion

namespace Runtime.Common
{
    public static class CommonVariable
    {
        public static float PercentageOf(this float check, float max)
        {
            return check / (max / 100);
        }
    }
}