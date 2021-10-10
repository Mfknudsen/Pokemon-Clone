using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input.Pokémon
{
    [System.Serializable]
    [Node("Input/Pokemon/Get Local Memories", "Get Local Memories")]
    public class GetLocalMemoriesNode : InputNode
    {
        [OutputType("Local Memories Input", typeof(LocalMemories))]
        public LocalMemories localMemories;

        public override void Tick(BattleAI setup)
        {
            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}