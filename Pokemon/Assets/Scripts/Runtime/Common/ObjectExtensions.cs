#region Packages

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Runtime.Common
{
    public static class ObjectExtensions
    {
        public static bool IsNull(this object o) => o == null;
    }
}