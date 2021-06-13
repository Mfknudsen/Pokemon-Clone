namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input.PokemonNodes
{
    public class GetPokemon : InputNode
    {
        [OutputType(VariableType.Script, "Input", ScriptType.Pokemon)]
        public object input = null;
    }
}
