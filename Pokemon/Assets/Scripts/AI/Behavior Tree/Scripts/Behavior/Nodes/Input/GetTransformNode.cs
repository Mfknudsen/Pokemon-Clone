using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input
{
    [System.Serializable]
    public class GetTransformNode : InputNode
    {
        [OutputType(VariableType.Transform, "Transform Input"), SerializeField]
        public object input = (Transform)null;

        public override void Tick(BehaviorController setup)
        {
            ContinueTransitions(setup);
        }
    }
}