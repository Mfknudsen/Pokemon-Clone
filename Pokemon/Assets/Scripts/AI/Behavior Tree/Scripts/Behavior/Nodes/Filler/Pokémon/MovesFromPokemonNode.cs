using Mfknudsen.Battle.Actions.Move;
using Mfknudsen.Pokémon;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Pokémon
{
    [System.Serializable]
    [Node("Selection/Pokemon/Moves From Pokemon", "Moves From Pokemon")]
    public class MovesFromPokemonNode : BaseNode
    {
        [InputType("Input", typeof(Pokemon))] public Pokemon input;

        [OutputType("Output", typeof(PokemonMove[]))]
        public PokemonMove[] output;

        public override void Tick(BehaviorController setup)
        {
            output = input.GetMoves();
        }

        protected override void Resets()
        {
            input = null;
            output = null;
        }
    }
}