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

        [SerializeField, HideInInlineEditors] private int id;

        [SerializeField, MinValue(.1f)] private float radius = .5f,
            height = 1,
            moveSpeed = 1,
            turnSpeed = 1,
            walkTurnAngle = 45f,
            stoppingDistance = .5f;

        [SerializeField] private List<int> usableNavigationPoints = new List<int>();

        #endregion

        #region Getters

        public int ID => this.id;

        public float Radius => this.radius;

        public float Height => this.height;

        public float MoveSpeed => this.moveSpeed;

        public float TurnSpeed => this.turnSpeed;

        public float WalkTurnAngle => this.walkTurnAngle;

        public float StoppingDistance => this.stoppingDistance;

        public List<int> GetUsableNavigationPoints => this.usableNavigationPoints;

        #endregion
    }
}