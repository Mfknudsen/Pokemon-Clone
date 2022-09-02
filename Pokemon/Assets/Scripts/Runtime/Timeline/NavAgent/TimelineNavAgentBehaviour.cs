#region Packages

using Runtime.Timeline.Mono;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

#endregion

namespace Runtime.Timeline.NavAgent
{
    public class TimelineNavAgentBehaviour : TimelineMonoBehaviour
    {
        #region Values

        public Transform toMoveTo;
        public bool isStopped;
        public float stopDistance;

        #endregion
        
        #region Build In States

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);
            
            NavMeshAgent navMeshAgent = playerData as NavMeshAgent;

            if(navMeshAgent == null) return;
            
            if (toMoveTo != null)
                navMeshAgent.SetDestination(toMoveTo.position);

            navMeshAgent.isStopped = isStopped;
            navMeshAgent.stoppingDistance = stopDistance;
        }

        #endregion       
    }
}