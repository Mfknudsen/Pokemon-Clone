namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input.PokemonNodes
{
    [System.Serializable]
    [Node("Input/Pokemon/Pokemon", "Pokemon Input")]
    public class GetPokemon : InputNode
    {
        [OutputType(VariableType.Script, "Input", ScriptType.Pokemon)]
        public object input = null;
    }
}
