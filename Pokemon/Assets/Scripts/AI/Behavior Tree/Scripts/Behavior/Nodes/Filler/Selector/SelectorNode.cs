using System.Collections.Generic;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Selector
{
    public class SelectorNode : BaseNode
    {
        [InputType(VariableType.Bool, "State", false)]
        public bool state;

        [OutCaller("True")]
        public List<Transition> outOne;
        [OutCaller("False")]
        public List<Transition> outTwo;

        public SelectorNode()
        {
            inCall = true;
        }

        public override void Tick(BehaviorController setup)
        {
            ContinueTransitions(setup);
        }
    }
}