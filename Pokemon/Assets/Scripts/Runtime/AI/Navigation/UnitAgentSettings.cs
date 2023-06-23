#region Libraries

using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Runtime.AI.Navigation
{
    [CreateAssetMenu(menuName = "AI/Agent Settings", fileName = "Agent")]
    public sealed class UnitAgentSettings : ScriptableObject
    {
        #region Values

        [SerializeField]
        private int id;

        [SerializeField, MinValue(.1f)]
        private float radius = .5f, height = 1, moveSpeed = 1, turnSpeed = 1, walkTurnAngle = 45f;

        [SerializeField]
        private readonly List<int> usableNavigationPoints = new();

        #endregion

        #region Getters

        public int ID => this.id;

        public float Radius => this.radius;

        public float Height => this.height;

        public float MoveSpeed => this.moveSpeed;

        public float TurnSpeed => this.turnSpeed;

        public float WalkTurnAngle => this.walkTurnAngle;

        public List<int> GetUsableNavigationPoints => this.usableNavigationPoints;

        #endregion
    }
}