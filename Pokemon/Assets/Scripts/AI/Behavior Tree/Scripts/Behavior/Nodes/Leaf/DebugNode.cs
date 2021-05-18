using AI.BehaviourTreeEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    [System.Serializable]
    public class DebugNode : LeafNode
    {
        [InputType(VariableType.Any, "To Log", true)]
        public object toLog;

        public override void Tick(BehaviorController setup)
        {
            Debug.Log("Debug Node Message:\n" + toLog);
        }
    }
}