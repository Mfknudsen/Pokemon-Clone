#region Packages

using NodeCanvas.Framework;
using UnityEngine;
using UnityEngine.AI;

#endregion

namespace Runtime.AI.Node_Canvas.Conditions
{
    public class CheckObjectState<T> : ConditionTask<NavMeshAgent> where T : Object
    {
        public BBParameter<string> key;
        public BBParameter<T> checkAgainst;

        protected override bool OnCheck()
        {
            return checkAgainst.value.Equals(agent.GetComponent<NpcController>().GetStateByKey(key.GetValue()));
        }
    }
}