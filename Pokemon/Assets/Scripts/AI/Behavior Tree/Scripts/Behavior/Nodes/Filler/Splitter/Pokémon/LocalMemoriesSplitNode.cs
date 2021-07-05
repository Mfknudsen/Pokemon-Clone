using Mfknudsen.Pokémon;
using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Splitter.Pokémon
{
    [System.Serializable]
    [Node("Splitter/Pokemon/Local Memories", "Local Memories Split")]
    public class LocalMemoriesSplitNode : BaseNode
    {
        [InputType("Input", typeof(LocalMemories))]
        public LocalMemories input;

        [OutputType("Pokemon", typeof(Pokemon))]
        public Pokemon pokemon;
        
        public override void Tick(BattleAI setup)
        {
            pokemon = input.currentPokemon;
            
            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}