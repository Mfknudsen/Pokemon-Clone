#if UNITY_EDITOR

#region Packages

using Runtime.World.Overworld;
using UnityEditor;
using UnityEngine;

#endregion

namespace Runtime.Editor.Tools
{
    public static class WorldTools
    {
        #region Internal

        [MenuItem("Tools/Mfknudsen/Setup World Scene")]
        private static void SetupNewScene()
        {
            GameObject obj = new GameObject
            {
                name = "'Name' - Tile Manager"
            };

            TileSubController tileSubController = obj.AddComponent<TileSubController>();
        }

        #endregion
    }
}
#endif