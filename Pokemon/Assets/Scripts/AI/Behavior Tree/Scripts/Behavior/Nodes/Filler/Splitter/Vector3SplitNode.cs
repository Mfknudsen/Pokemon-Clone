using AI.BehaviorTree;
using AI.BehaviorTree.Nodes;
using AI.BehaviourTreeEditor;
using UnityEngine;

namespace AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Splitter
{
    public class Vector3SplitNode : BaseNode
    {
        [InputType(VariableType.Vector3, "Vector3 Input", true)]
        public object input = Vector3.zero;

        [OutputType(VariableType.Float, "X")] public object x = 0.0f;
        [OutputType(VariableType.Float, "Y")] public object y = 0.0f;
        [OutputType(VariableType.Float, "Z")] public object z = 0.0f;

        public override void Tick(BehaviorController setup)
        {
            Vector3 vec = (Vector3) input;

            x = vec.x;
            y = vec.y;
            z = vec.z;

            ContinueTransitions(setup);
        }
    }
}