using System.Collections.Generic;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Selector
{
    [System.Serializable]
    [Node("Filler/Selection/Selector", "Selector")]
    public class SelectorNode : BaseNode
    {
        [InputType("State", typeof(bool))]
        public bool state;

        [OutCaller("True")]
        public List<Transition> outOne;
        [OutCaller("False")]
        public List<Transition> outTwo;

        public SelectorNode()
        {
            inCall = true;
        }

        public override void Tick(BattleAI setup)
        {
            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}