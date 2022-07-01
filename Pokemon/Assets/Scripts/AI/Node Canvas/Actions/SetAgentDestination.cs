#region Packages

using System.Collections.Generic;
using NodeCanvas.Framework;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Mfknudsen.AI.Node_Canvas.Actions
{
    public class SetAgentDestination : ActionTask
    {
        public BBParameter<NavMeshAgent> navAgent;
        public BBParameter<List<Transform>> waypoints;
        public BBParameter<int> index;

        protected override void OnExecute()
        {
            navAgent.GetValue().isStopped = false;
            navAgent.GetValue().SetDestination(waypoints.value[index.value].position);
            
            EndAction(true);
        }
    }
}