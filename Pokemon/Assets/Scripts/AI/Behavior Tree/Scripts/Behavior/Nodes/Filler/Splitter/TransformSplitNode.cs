﻿using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Splitter
{
    [System.Serializable]
    [Node("Splitter/Transform", "Transform Split")]
    public class TransformSplitNode : BaseNode
    {
        [InputType(VariableType.Transform, "Transform Input")]
        public object input = null;

        [OutputType(VariableType.Vector3, "Position")]
        public object pos = Vector3.zero;

        [OutputType(VariableType.Vector3, "Rotation")]
        public object rot = Vector3.zero;

        public override void Tick(BehaviorController setup)
        {
            pos = (input as Transform).position;

            rot = (input as Transform).rotation.eulerAngles;
            
            ContinueTransitions(setup);
        }
    }
}