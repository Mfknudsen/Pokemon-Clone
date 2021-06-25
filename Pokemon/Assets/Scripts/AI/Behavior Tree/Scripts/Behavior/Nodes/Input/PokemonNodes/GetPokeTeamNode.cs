namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input.PokemonNodes
{
    [System.Serializable]
    [Node("Input/Pokemon/Team", "Pokemon Team Input")]
    public class GetPokeTeamNode : InputNode
    {
        [OutputType(VariableType.Script, "Team Input", ScriptType.TrainerTeam)]
        public object input = (Trainer.Team)null;

        public override void Tick(BehaviorController setup)
        {
            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}
