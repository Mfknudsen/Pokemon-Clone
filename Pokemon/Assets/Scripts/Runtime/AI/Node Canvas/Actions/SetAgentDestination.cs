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
            this.navAgent.GetValue().isStopped = false;
            this.navAgent.GetValue().SetDestination(this.target.value.position);

            this.EndAction();
        }
    }
}