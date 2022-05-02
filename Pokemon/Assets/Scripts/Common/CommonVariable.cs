#region Packages

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Mfknudsen.Common
{
    public static class CommonVariable
    {
        public static float PercentageOf(float check, float max)
        {
            return check / (max / 100);
        }
    }
}