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
        private readonly List<int> usableNavigationPoints = new();

        #endregion

        #region Getters

        public int ID => this.id;

        public float Radius => this.radius;

        public float MoveSpeed => this.moveSpeed;

        public float TurnSpeed => this.turnSpeed;

        public List<int> GetUsableNavigationPoints => this.usableNavigationPoints;

        #endregion
    }
}