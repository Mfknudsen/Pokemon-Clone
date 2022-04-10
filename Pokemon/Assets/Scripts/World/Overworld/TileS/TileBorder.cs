#region Packages

using UnityEngine;

#endregion

namespace Mfknudsen.World.Overworld.TileS
{
    public class TileBorder : MonoBehaviour
    {
        #region Values

        [SerializeField] private string fromName, toName;

        #endregion

        #region In

        public void Trigger(bool exitCurrentTile)
        {
            TileManager.instance.SetCurrentSubTile(exitCurrentTile ? toName : fromName);
        }

        #endregion
    }
}