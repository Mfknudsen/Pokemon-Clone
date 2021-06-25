namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input.PokemonNodes
{
    [System.Serializable]
    [Node("Input/Pokemon/Pokemon", "Pokemon Input")]
    public class GetPokemon : InputNode
    {
        [OutputType(VariableType.Script, "Input", ScriptType.Pokemon)]
        public object input = null;

        public override void Tick(BehaviorController setup)
        {
            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}
