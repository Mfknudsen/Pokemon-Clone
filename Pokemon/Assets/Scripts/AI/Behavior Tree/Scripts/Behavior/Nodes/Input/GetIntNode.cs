using UnityEngine; //Custom

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input
{
    [System.Serializable]
    public class GetIntNode : InputNode
    {
        [OutputType(VariableType.Int, "Int Input"), SerializeReference]
        public object input = 0;

        public override void Tick(BehaviorController setup)
        {
            ContinueTransitions(setup);
        }
    }
}