#region Packages

using NodeCanvas.Framework;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Runtime.AI.Node_Canvas.Actions
{
    public class SetAgentDestination : ActionTask
    {
        public BBParameter<NavMeshAgent> navAgent;
        public BBParameter<Transform> target;

        protected override void OnExecute()
        {
            navAgent.GetValue().isStopped = false;
            navAgent.GetValue().SetDestination(target.value.position);
            
            EndAction();
        }
    }
}