#region Packages

using NodeCanvas.Framework;
using UnityEngine.AI;

#endregion

namespace Mfknudsen.AI.Node_Canvas.Actions
{
    public class StopAgent : ActionTask
    {
        public BBParameter<NavMeshAgent> navAgent;

        protected override void OnExecute()
        {
            navAgent.GetValue().isStopped = true;
            
            EndAction(true);
        }
    }
}