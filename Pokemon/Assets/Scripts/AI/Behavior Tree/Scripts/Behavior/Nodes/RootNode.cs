using System.Collections.Generic;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes
{
    [System.Serializable]
    [Node("Root", "Root", 50)]
    public class RootNode : BaseNode
    {
        [OutCaller("")]
        public List<Transition> outCalls;
        public override void Tick(BattleAI setup)
        {
            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}