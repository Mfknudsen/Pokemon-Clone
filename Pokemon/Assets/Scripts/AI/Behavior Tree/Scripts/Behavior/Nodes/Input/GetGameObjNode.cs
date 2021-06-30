using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input
{
    [Node("Input/GameObject", "GameObject Input")]
    public class GetGameObjNode : BaseNode
    {
        [OutputType("GameObject Input", typeof(GameObject), true)]
        public GameObject input = null;
        
        public override void Tick(BehaviorController setup)
        {
            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}
