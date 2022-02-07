#region Packages

using Mfknudsen.Common;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Mfknudsen.AI.States.Movement
{
    public class SearchState : NpcMoveState
    {
        private Transform waypoint;
        private NavMeshAgent agent;
        private Timer timer;
        private int searchCount;
        private float timeToSearch;

        public SearchState(NpcBase npcBase, Transform waypoint, float timeToSearch, int searchCount) : base(npcBase)
        {
            this.waypoint = waypoint;
            this.searchCount = searchCount;
            this.timeToSearch = timeToSearch;

            agent = npcBase.GetAgent();
            agent.SetDestination(waypoint.position);
        }

        public override void Update()
        {
            if (agent.remainingDistance < 1)
            {
                agent.isStopped = true;

                if (searchCount == 0)
                {
                    return;
                }

                timer = new Timer(timeToSearch);
                timer.timerEvent.AddListener(() =>
                {
                    searchCount--;
                    RandomPosAroundAgent();
                });
            }
        }

        #region Internal

        private void RandomPosAroundAgent()
        {
            NavMeshHit hit;
            NavMesh.SamplePosition(
                agent ? agent.transform.position : waypoint.position,
                out hit,
                Random.Range(2, 5),
                0);
            agent.SetDestination(hit.position);
        }

        #endregion
    }
}