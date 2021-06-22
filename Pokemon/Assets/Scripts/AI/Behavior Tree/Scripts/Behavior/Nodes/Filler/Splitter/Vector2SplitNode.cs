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
        public object input = Vector2.zero;

        [OutputType(VariableType.Float, "X")] public object x = 0.0f;
        [OutputType(VariableType.Float, "Y")] public object y = 0.0f;

        public override void Tick(BehaviorController setup)
        {
            Vector2 vec = (Vector2) input;

            x = vec.x;

            y = vec.y;

            ContinueTransitions(setup);
        }
    }
}