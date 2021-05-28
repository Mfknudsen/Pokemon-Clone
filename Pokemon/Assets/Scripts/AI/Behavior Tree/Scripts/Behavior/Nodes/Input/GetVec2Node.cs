using AI.BehaviourTreeEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    public class GetVec2Node : InputNode
    {
        [OutputType(VariableType.Vector2, "Vector2 Input")]
        public object input = Vector2.zero;
        
        public override void Tick(BehaviorController setup)
        {
            ContinueTransitions(setup);
        }
    }
}