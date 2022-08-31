#region Packages

using UnityEditor;
using UnityEngine;

#endregion

namespace Runtime.Editor.Tools
{
    public static class WorldTools
    {
        #region Internal

        [MenuItem("Tools/Setup World Scene")]
        private static void SetupNewScene()
        {
            GameObject obj = new()
            {
                name = "'Name' - Tile Manager"
            };
        }

        #endregion
    }
}