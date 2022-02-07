#region Packages

using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Mfknudsen.AI.States.Movement
{
    public enum PatrolType
    {
        FromTo,
        Around
    }

    public class PatrolState : NpcMoveState
    {
        private Transform currentWaypoint;
        private Transform[] waypoints;
        private PatrolType patrolType;
        private NavMeshAgent agent;
        private int arrayDir = 1, currentIndex;

        public PatrolState(NpcBase npcBase, Transform[] waypoints, PatrolType patrolType) : base(npcBase)
        {
            this.waypoints = waypoints;
            this.patrolType = patrolType;

            agent = npcBase.GetAgent();
        }

        public override void Update()
        {
            if (currentWaypoint == null)
            {
                if (currentIndex == waypoints.Length || currentIndex == 0)
                {
                    if (patrolType == PatrolType.FromTo)
                    {
                        currentIndex--;
                        arrayDir *= -1;
                    }
                    else if (patrolType == PatrolType.Around)
                        currentIndex = 0;

                    currentWaypoint = waypoints[currentIndex];
                }
                else
                {
                    currentIndex += arrayDir;
                    currentWaypoint = waypoints[currentIndex];
                }

                agent.SetDestination(currentWaypoint.position);
            }

            if (agent.remainingDistance < 1)
                currentWaypoint = null;
        }
    }
}