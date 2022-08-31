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
            if (navAgent.value != null && controller.value != null)
            {
                EndAction(true);
                return;
            }
            
            try
            {
                navAgent.value = agent;

                controller.value = agent.GetComponent<NpcController>();
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