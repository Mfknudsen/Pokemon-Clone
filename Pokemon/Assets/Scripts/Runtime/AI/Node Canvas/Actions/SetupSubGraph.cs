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
                this.EndAction(true);
                return;
            }
            
            try
            {
                this.navAgent.value = this.agent;

                this.controller.value = this.agent.GetComponent<NpcController>();
            }
            catch
            {
                this.EndAction(false);
                return;
            }

            this.EndAction(true);
        }
    }
}