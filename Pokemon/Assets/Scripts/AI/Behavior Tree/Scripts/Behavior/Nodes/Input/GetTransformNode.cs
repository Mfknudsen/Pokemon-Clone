using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input
{
    [System.Serializable]
    [Node("Input/Transform", "Transform Input")]
    public class GetTransformNode : InputNode
    {
        [OutputType("Transform Input", typeof(Transform), true)]
        public Transform input = null;

        public override void Tick(BehaviorController setup)
        {
            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}