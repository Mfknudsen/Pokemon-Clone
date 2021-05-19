using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI.BehaviourTreeEditor;

namespace AI.BehaviorTree.Nodes
{
    [System.Serializable]
    public class GetFloatNode : InputNode
    {
        [OutputType(VariableType.Float, "Float Input")]
        public object input = 0.0f;

        public override void Tick(BehaviorController setup)
        {
            ContinueTransitions(setup);
        }
    }
}