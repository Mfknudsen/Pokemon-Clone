using System.Collections.Generic;
using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Math.Trans
{
    [System.Serializable]
    [Node("Transform/Rotate Transform", "Rotate")]
    public class RotateTransformNode : BaseNode
    {
        [OutCaller("")] public List<Transition> outCalls;

        [InputType(VariableType.Transform, "Target"), OutputType(VariableType.Transform, "Result")]
        public Transform target = null;

        [InputType(VariableType.Vector3, "Amount")]
        public Vector3 amount = Vector3.zero;
 
        public RotateTransformNode()
        {
            inCall = true;
            outCalls = new List<Transition>();
        }

        public override void Tick(BehaviorController setup)
        {
            Transform transform = target;

            transform.Rotate(amount * Time.deltaTime);

            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}