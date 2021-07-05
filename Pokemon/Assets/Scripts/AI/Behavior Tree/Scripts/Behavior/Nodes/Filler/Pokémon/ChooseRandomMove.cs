using Mfknudsen.Battle.Actions.Move;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Pokémon
{
    [System.Serializable]
    [Node("Selection/Pokemon/Random Move", "Choose Random Move")]
    public class ChooseRandomMove : BaseNode
    {
        [InputType("Moves", typeof(PokemonMove[]))]
        public PokemonMove[] moves;

        [OutputType("Selection", typeof(PokemonMove))]
        public PokemonMove selection;

        public override void Tick(BattleAI setup)
        {
            List<PokemonMove> result = moves.Where(pokemonMove => pokemonMove != null).ToList();
            
            int i = Random.Range(0, result.Count);
            
            selection = result[i];

            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}