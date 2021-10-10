using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input
{
    [System.Serializable]
    [Node("Input/Vector2", "Vector2 Input")]
    public class GetVec2Node : InputNode
    {
        [OutputType("Vector2 Input", typeof(Vector2), true)]
        public Vector2 input = Vector2.zero;
        
        public override void Tick(BattleAI setup)
        {
            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}