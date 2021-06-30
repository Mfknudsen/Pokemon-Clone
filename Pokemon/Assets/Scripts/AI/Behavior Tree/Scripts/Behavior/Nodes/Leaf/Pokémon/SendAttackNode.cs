using Mfknudsen.Battle.Actions.Move;
using Mfknudsen.Battle.Systems;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Leaf.Pokémon
{
    [System.Serializable]
    [Node("Leaf/Pokemon/Send Attack", "Send Pokemon Attack")]
    public class SendAttackNode : BaseNode
    {
        [InputType("Move", typeof(PokemonMove))]
        public PokemonMove move;

        [InputType("Target Spot Index", typeof(int))]
        public int targetSpotIndex;

        public SendAttackNode()
        {
            inCall = true;
        }

        public override void Tick(BehaviorController setup)
        {
            Decision result = new Decision(move, targetSpotIndex);

            move.SetTargetIndex(targetSpotIndex);

            BattleMaster.instance.GetComputerAction(result);

            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}