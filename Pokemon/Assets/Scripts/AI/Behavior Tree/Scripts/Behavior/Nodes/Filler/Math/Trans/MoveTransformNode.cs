#region SDK

using System.Collections.Generic;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Leaf;
using UnityEngine;

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Math.Trans
{
    [System.Serializable]
    [Node("Transform/Move Transform", "Move Transform")]
    public class MoveTransformNode : BaseNode
    {
        [OutCaller("")] public List<Transition> outCalls;

        [InputType("Transform Input", typeof(Transform)), OutputType("Result", typeof(Transform))]
        public Transform input = null;

        [InputType("Position", typeof(Vector3))]
        public Vector3 pos = Vector3.zero;

        public MoveTransformNode()
        {
            inCall = true;
            outCalls = new List<Transition>();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public override void Tick(BattleAI setup)
        {
            if (input == null)
                Debug.LogError("Cannot Move Null Transform");

            input.position += pos * Time.deltaTime;

            ContinueTransitions(setup);
        }

        protected override void Resets()
        {
        }
    }
}