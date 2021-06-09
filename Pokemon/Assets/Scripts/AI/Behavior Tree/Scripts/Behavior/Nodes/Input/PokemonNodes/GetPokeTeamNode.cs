namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input.PokemonNodes
{
    public class GetPokeTeamNode : InputNode
    {
        [OutputType(VariableType.Script, "Team Input", ScriptType.TrainerTeam)]
        public object input = (Trainer.Team)null;
    }
}
