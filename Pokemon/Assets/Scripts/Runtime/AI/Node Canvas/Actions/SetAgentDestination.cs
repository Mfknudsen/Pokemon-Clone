#region Packages

using NodeCanvas.Framework;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Runtime.AI.Node_Canvas.Actions
{
    public class SetAgentDestination : ActionTask<NavMeshAgent>
    {
        public BBParameter<Transform> target;

        protected override void OnExecute()
        {
            NavMeshAgent navAgent = this.agent;
            
            navAgent.isStopped = false;
            navAgent.SetDestination(this.target.value.position);

            this.EndAction(true);
        }
    }
}