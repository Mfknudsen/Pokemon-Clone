using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Leaf;
using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Math.Trans
{
    [System.Serializable]
    [Node("Transform/Move Transform", "Move Transform")]
    public class MoveTransformNode : LeafNode
    {
        [InputType(VariableType.Transform, "Transform Input")]
        public Transform input = null;

        [InputType(VariableType.Vector3, "Position")]
        public Vector3 pos = Vector3.zero;
        
        public override void Tick(BehaviorController setup)
        {
            if (input == null)
            {
                Debug.LogError("Cannot Move Null Transform");
            }
            
            input.position += pos * Time.deltaTime;
            
            ContinueTransitions(setup);
        }
    }
}