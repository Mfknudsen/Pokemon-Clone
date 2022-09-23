#region Packages

using NodeCanvas.Framework;
using UnityEngine.AI;

#endregion

namespace Runtime.AI.Node_Canvas.Actions
{
    public class SetupSubGraph : ActionTask<NavMeshAgent>
    {
        public BBParameter<NavMeshAgent> navAgent;
        public BBParameter<NpcController> controller;

        protected override void OnExecute()
        {
            if (this.navAgent.value != null && this.controller.value != null)
            {
                EndAction(true);
                return;
            }
            
            try
            {
                this.navAgent.value = agent;

                this.controller.value = agent.GetComponent<NpcController>();
            }
            catch
            {
                EndAction(false);
                return;
            }

            EndAction(true);
        }
    }
}