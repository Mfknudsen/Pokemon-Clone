#region SDK

using AI.BehaviorTree;
using AI.BehaviorTree.Nodes;
using AI.BehaviourTreeEditor;
using UnityEngine;

#endregion

namespace AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Math
{
    [System.Serializable]
    public class ClampNode : BaseNode
    {
        [InputType(VariableType.Float, "To Check", true)]
        public object toCheck = 0.0f;

        [InputType(VariableType.Float, "Min", false), SerializeReference]
        public object min = 0.0f;

        [InputType(VariableType.Float, "Max", false), SerializeReference]
        public object max = 0.0f;

        [OutputType(VariableType.Float, "Result"), SerializeReference]
        public object result = 0.0f;

        public override void Tick(BehaviorController setup)
        {
            result = Mathf.Clamp((float) toCheck, (float) min, (float) max);

            ContinueTransitions(this, setup);
        }
    }
}