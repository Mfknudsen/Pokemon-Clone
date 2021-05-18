#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Custom
#endregion

namespace NPC
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