using System.Collections.Generic;
using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Math.Trans
{
    [System.Serializable]
    [Node("Transform/Rotate Transform", "Rotate")]
    public class RotateTransformNode : BaseNode
    {
        [OutCaller("")] public List<Transition> outCalls;

        [InputType("Target", typeof(Transform)), OutputType("Result", typeof(Transform))]
        public Transform target = null;

        [InputType("Amount", typeof(Vector3))]
        public Vector3 amount = Vector3.zero;
 
        public RotateTransformNode()
        {
            inCall = true;
            outCalls = new List<Transition>();
        }

        public override void Tick(BattleAI setup)
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