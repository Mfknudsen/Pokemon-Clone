using System.Collections.Generic;
using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Math
{
    [System.Serializable]
    public class RotateNode : BaseNode
    {
        [OutCaller("")] public List<Transition> outCalls;

        [InputType(VariableType.Transform, "Target", true), OutputType(VariableType.Transform, "Result")]
        public object target = null;

        [InputType(VariableType.Vector3, "Amount", true)]
        public object amount = Vector3.zero;

        public RotateNode()
        {
            inCall = true;
            outCalls = new List<Transition>();
        }

        public override void Tick(BehaviorController setup)
        {
            Transform transform = (Transform) target;

            transform.Rotate((Vector3) amount * Time.deltaTime);

            ContinueTransitions(setup);
        }
    }
}