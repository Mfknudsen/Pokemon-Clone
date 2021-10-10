using Mfknudsen.Battle.Actions;
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

        public SendAttackNode()
        {
            inCall = true;
        }

        public override void Tick(BattleAI setup)
        {
            pokemon.SetBattleAction(move);

            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}