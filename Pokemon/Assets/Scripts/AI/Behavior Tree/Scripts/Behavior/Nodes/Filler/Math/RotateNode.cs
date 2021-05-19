using AI.BehaviourTreeEditor;
using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    [System.Serializable]
    public class RotateNode : BaseNode
    {
        [InputType(VariableType.Transform, "Target", true), OutputType(VariableType.Transform, "Result")]
        public object target = null;

        [InputType(VariableType.Vector3, "Amount", true)]
        public object amount = Vector3.zero;

        public override void Tick(BehaviorController setup)
        {
            Transform transform = (Transform) target;

            transform.Rotate((Vector3) amount * Time.deltaTime);

            ContinueTransitions(setup);
        }
        
        
    }
}