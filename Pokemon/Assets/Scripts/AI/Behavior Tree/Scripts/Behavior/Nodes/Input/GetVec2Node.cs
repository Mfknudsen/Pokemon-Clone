using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input
{
    [System.Serializable]
    [Node("Input/Vector2", "Vector2 Input")]
    public class GetVec2Node : InputNode
    {
        [OutputType(VariableType.Vector2, "Vector2 Input")]
        public Vector2 input = Vector2.zero;
        
        public override void Tick(BehaviorController setup)
        {
            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}