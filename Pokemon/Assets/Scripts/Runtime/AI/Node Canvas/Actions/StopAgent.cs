#region Packages

using NodeCanvas.Framework;
using UnityEngine.AI;

#endregion

namespace Runtime.AI.Node_Canvas.Actions
{
    public class StopAgent : ActionTask
    {
        public BBParameter<NavMeshAgent> navAgent;

        protected override void OnExecute()
        {
            this.navAgent.GetValue().isStopped = true;

            this.EndAction(true);
        }
    }
}