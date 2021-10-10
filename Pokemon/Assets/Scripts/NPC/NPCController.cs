#region SDK

using UnityEngine;

//Custom
#endregion

namespace Mfknudsen.NPC
{
    public class NPCController : MonoBehaviour
    {
        #region Values
        [SerializeField] int id = 0;
        [SerializeField] private string characterName = "";

        private void OnValidate()
        {

        }
        #endregion
    }
}