using UnityEngine; 

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input
{
    [System.Serializable]
    [Node("Input/Int", "Int Input")]
    public class GetIntNode : InputNode
    {
        [OutputType("Int Input", typeof(int), true), SerializeReference]
        public int input = 0;

        public override void Tick(BehaviorController setup)
        {
            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}