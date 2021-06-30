using Mfknudsen.Pokémon;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input.Pokémon
{
    [System.Serializable]
    [Node("Input/Pokemon/Pokemon", "Pokemon Input")]
    public class GetPokemon : InputNode
    {
        [OutputType("Input", typeof(Pokemon), true)]
        public Pokemon input = null;

        public override void Tick(BehaviorController setup)
        {
            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}
