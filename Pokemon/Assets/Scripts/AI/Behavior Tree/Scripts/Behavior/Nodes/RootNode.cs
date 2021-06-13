using System.Collections.Generic;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes
{
    [System.Serializable]
    public class RootNode : BaseNode
    {
        [OutCaller("")]
        public List<Transition> outCalls;
        public override void Tick(BehaviorController setup)
        {
            ContinueTransitions(setup);
        }
    }
}