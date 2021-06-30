#region SDK

using UnityEngine;

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Splitter
{
    [System.Serializable]
    [Node("Splitter/Vector2", "Vector2 Split")]
    public class Vector2SplitNode : BaseNode
    {
        [InputType("Input", typeof(Vector2))] public Vector2 input = Vector2.zero;

        [OutputType("X", typeof(float))] public float x = 0.0f;
        [OutputType("Y", typeof(float))] public float y = 0.0f;

        public override void Tick(BehaviorController setup)
        {
            Vector2 vec = input;

            x = vec.x;

            y = vec.y;

            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
            x = 0.0f;
            y = 0.0f;
        }
    }
}