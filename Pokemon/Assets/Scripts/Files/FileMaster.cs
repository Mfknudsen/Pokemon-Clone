#region SDK

using UnityEngine;

#endregion

namespace Mfknudsen.Files
{
    [ExecuteInEditMode]
    public class FileMaster : MonoBehaviour
    {
        #region Values
        [Header("Object Refernce")]
        public static FileMaster instance = null;
        public bool UPDATE = false;
        #endregion

        private void Update()
        {
            if (UPDATE)
            {

                UPDATE = false;
            }
        }

        #region Player
        public static void SavePlayerData(Player.MasterPlayer player)
        {

        }

        public static void LoadDataToPlayer(Player.MasterPlayer player)
        {

        }
        #endregion

        #region Pokemon()
        #endregion
    }
}
