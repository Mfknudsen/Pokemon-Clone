using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Splitter
{
    [System.Serializable]
    [Node("Splitter/Vector3", "Vector3 Split")]
    public class Vector3SplitNode : BaseNode
    {
        [InputType(VariableType.Vector3, "Vector3 Input")]
        public Vector3 input = Vector3.zero;

        [OutputType(VariableType.Float, "X")] public float x = 0.0f;
        [OutputType(VariableType.Float, "Y")] public float y = 0.0f;
        [OutputType(VariableType.Float, "Z")] public float z = 0.0f;

        public override void Tick(BehaviorController setup)
        {
            Vector3 vec = input;

            x = vec.x;
            y = vec.y;
            z = vec.z;

            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
            x = 0.0f;
            y = 0.0f;
            z = 0.0f;
        }
    }
}