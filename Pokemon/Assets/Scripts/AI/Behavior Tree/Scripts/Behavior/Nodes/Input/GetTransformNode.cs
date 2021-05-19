using AI.BehaviorTree;
using AI.BehaviorTree.Nodes;
using AI.BehaviourTreeEditor;
using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    [System.Serializable]
    public class GetTransformNode : InputNode
    {
        [OutputType(VariableType.Transform, "Transform Input")]
        public object input = null;

        public override void Tick(BehaviorController setup)
        {
            ContinueTransitions(setup);
        }
    }
}