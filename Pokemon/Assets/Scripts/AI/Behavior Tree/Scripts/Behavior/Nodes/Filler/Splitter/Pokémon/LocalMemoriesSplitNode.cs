#region SDK

using Mfknudsen.Battle.Systems;
using Mfknudsen.Pokémon;

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Splitter.Pokémon
{
    [System.Serializable]
    [Node("Splitter/Pokemon/Local Memories", "Local Memories Split")]
    public class LocalMemoriesSplitNode : BaseNode
    {
        [InputType("Input", typeof(LocalMemories))]
        // ReSharper disable once UnassignedField.Global
        public LocalMemories input;

        [OutputType("Pokemon", typeof(Pokemon))]
        public Pokemon pokemon;

        [OutputType("Spot", typeof(Spot))] public Spot spot;

        public override void Tick(BattleAI setup)
        {
            pokemon = input.currentPokemon;
            spot = input.currentSpot;

            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}