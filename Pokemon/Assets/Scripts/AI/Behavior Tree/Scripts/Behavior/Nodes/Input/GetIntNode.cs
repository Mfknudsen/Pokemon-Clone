using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Custom
using AI.BehaviourTreeEditor;

namespace AI.BehaviorTree.Nodes
{
    [System.Serializable]
    public class GetIntNode : InputNode
    {
        [OutputType(VariableType.Int, "Int Input"), SerializeReference]
        public object input = 0;

        public override void Tick(BehaviorController setup)
        {
            ContinueTransitions(setup);
        }
    }
}