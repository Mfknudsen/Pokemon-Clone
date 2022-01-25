#region Packages

using UnityEngine;

#endregion

namespace Mfknudsen.World.Overworld.TileS
{
    public class TileDivider : MonoBehaviour
    {
        #region Values

        [SerializeField] private string fromName, toName;

        #endregion

        #region In

        public void Trigger(bool exitCurrentTile)
        {
            TileManager.instance.UpdateNavmesh(exitCurrentTile ? toName : fromName);
        }

        #endregion
    }
}