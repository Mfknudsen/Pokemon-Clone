#region SDK

using UnityEngine;

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input
{
    [System.Serializable]
    public class GetVec3Node : InputNode
    {
        [OutputType(VariableType.Vector3, "Vector3 Input"), SerializeReference]
        public object input = Vector3.zero;

        public override void Tick(BehaviorController setup)
        {
            ContinueTransitions(setup);
        }
    }
}