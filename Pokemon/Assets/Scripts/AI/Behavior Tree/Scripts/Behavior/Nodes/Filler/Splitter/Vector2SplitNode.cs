#region SDK

using UnityEngine;

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Splitter
{
    [System.Serializable]
    [Node("Splitter/Vector2", "Vector2 Split")]
    public class Vector2SplitNode : BaseNode
    {
        [InputType(VariableType.Vector2, "Input")]
        public Vector2 input = Vector2.zero;

        [OutputType(VariableType.Float, "X")] public float x = 0.0f;
        [OutputType(VariableType.Float, "Y")] public float y = 0.0f;

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