using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Splitter
{
    [System.Serializable]
    [Node("Splitter/Transform", "Transform Split")]
    public class TransformSplitNode : BaseNode
    {
        [InputType("Transform Input", typeof(Transform))]
        public Transform input = null;

        [OutputType("Position", typeof(Vector3))]
        public Vector3 pos = Vector3.zero;

        [OutputType("Rotation", typeof(Vector3))]
        public Vector3 rot = Vector3.zero;

        public override void Tick(BehaviorController setup)
        {
            pos = input.position;

            rot = input.rotation.eulerAngles;
            
            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
            throw new System.NotImplementedException();
        }
    }
}