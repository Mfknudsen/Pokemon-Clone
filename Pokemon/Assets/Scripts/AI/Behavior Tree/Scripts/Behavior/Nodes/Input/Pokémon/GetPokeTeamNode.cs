using Mfknudsen.Trainer;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input.Pokémon
{
    [System.Serializable]
    [Node("Input/Pokemon/Team", "Pokemon Team Input")]
    public class GetPokeTeamNode : InputNode
    {
        [OutputType("Team Input", typeof(Team), true)]
        public Team input = null;

        public override void Tick(BattleAI setup)
        {
            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}