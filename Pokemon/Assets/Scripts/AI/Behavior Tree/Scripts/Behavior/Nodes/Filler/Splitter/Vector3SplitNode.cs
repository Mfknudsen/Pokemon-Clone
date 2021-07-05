using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Splitter
{
    [System.Serializable]
    [Node("Splitter/Vector3", "Vector3 Split")]
    public class Vector3SplitNode : BaseNode
    {
        [InputType("Vector3 Input", typeof(Vector3))]
        public Vector3 input = Vector3.zero;

        [OutputType("X", typeof(float))] public float x = 0.0f;
        [OutputType("Y", typeof(float))] public float y = 0.0f;
        [OutputType("Z", typeof(float))] public float z = 0.0f;

        public override void Tick(BattleAI setup)
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