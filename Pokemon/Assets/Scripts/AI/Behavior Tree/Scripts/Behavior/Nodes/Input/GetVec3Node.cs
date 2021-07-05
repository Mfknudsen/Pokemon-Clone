#region SDK

using UnityEngine;

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input
{
    [System.Serializable]
    [Node("Input/Vector3", "Vector3 Input")]
    public class GetVec3Node : InputNode
    {
        [OutputType("Vector3 Input", typeof(Vector3), true), SerializeReference]
        public Vector3 input = Vector3.zero;

        public override void Tick(BattleAI setup)
        {
            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}