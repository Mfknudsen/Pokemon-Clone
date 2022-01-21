#region Packages

using System;
using Mfknudsen.NPC;
using UnityEngine;

#endregion

namespace Mfknudsen.AI
{
    public class NpcController : MonoBehaviour
    {
        #region Values

        [SerializeField] private string characterName = "";
        [SerializeField] private NpcTeam npcTeam;

        #endregion

        #region Build In States

        private void Awake()
        {
            NpcManager.Instance.AddController(this);
        }

        private void OnDestroy()
        {
            NpcManager.Instance.RemoveController(this);
        }

        #endregion
    }
}