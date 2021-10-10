using Mfknudsen.Battle.Actions;
using Mfknudsen.Pokémon;
using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Pokémon
{
    [System.Serializable]
    [Node("Selection/Pokemon/Moves From Pokemon", "Moves From Pokemon")]
    public class MovesFromPokemonNode : BaseNode
    {
        [InputType("Input", typeof(Pokemon))] public Pokemon input;

        [OutputType("Output", typeof(PokemonMove[]))]
        public PokemonMove[] output;

        public override void Tick(BattleAI setup)
        {
            output = input.GetMoves();
            
            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
            input = null;
            output = null;
        }
    }
}