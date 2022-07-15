#region Packages

using Mfknudsen.World.Overworld.Tiles;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;

#endregion

namespace Mfknudsen.Editor.Tools
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