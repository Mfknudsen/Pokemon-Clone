#region Libraries

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Runtime.AI.World.Navigation
{
    public sealed class UnitAgentSettings : ScriptableObject
    {
        #region Values

        [SerializeField]
        private int id;
        [SerializeField]
        private float radius, moveSpeed, turnSpeed;
        [SerializeField]
        private List<int> usableNavigationPoints;

        #endregion
    }
}