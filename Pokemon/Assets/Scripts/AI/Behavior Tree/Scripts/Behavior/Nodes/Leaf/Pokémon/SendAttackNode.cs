using Mfknudsen.Battle.Actions.Move;
using Mfknudsen.Pokémon;
using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Leaf.Pokémon
{
    [System.Serializable]
    [Node("Leaf/Pokemon/Send Attack", "Send Pokemon Attack")]
    public class SendAttackNode : LeafNode
    {
        [InputType("Pokemon", typeof(Pokemon))]
        public Pokemon pokemon;

        [InputType("Move", typeof(PokemonMove))]
        public PokemonMove move;

        [InputType("Target Spot Index", typeof(Vector2))]
        public Vector2 targetSpotIndex;

        public SendAttackNode()
        {
            inCall = true;
        }

        public override void Tick(BattleAI setup)
        {
            move.SetTargetIndex(targetSpotIndex);
            move.SetCurrentPokemon(pokemon);
            
            pokemon.SetBattleAction(move);

            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}