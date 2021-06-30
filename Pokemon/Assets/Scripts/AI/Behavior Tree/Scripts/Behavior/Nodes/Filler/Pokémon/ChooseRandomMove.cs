using Mfknudsen.Battle.Actions.Move;
using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Pokémon
{
    [Node("Selection/Pokemon/Random Move", "Choose Random Move")]
    public class ChooseRandomMove : BaseNode
    {
        [InputType("Moves", typeof(PokemonMove[]))]
        public PokemonMove[] moves;

        [OutputType("Selection", typeof(PokemonMove))]
        public PokemonMove selection;

        public override void Tick(BehaviorController setup)
        {
            int i = Random.Range(0, moves.Length);
            
            selection = moves[i];

            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}