#region Packages

using UnityEngine;

#endregion

namespace Mfknudsen.NPC
{
    public class NpcController : MonoBehaviour
    {
        #region Values

        [SerializeField] private string characterName = "";
        [SerializeField] private NpcTeam npcTeam;

        #endregion
    }
}