using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Splitter
{
    [System.Serializable]
    [Node("Splitter/Transform", "Transform Split")]
    public class TransformSplitNode : BaseNode
    {
        [InputType(VariableType.Transform, "Transform Input")]
        public Transform input = null;

        [OutputType(VariableType.Vector3, "Position")]
        public Vector3 pos = Vector3.zero;

        [OutputType(VariableType.Vector3, "Rotation")]
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