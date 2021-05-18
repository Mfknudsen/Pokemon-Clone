#region SDK

using System;
using System.Collections;
using System.Collections.Generic;
using AI.BehaviourTreeEditor;
using UnityEngine;

#endregion

namespace AI.BehaviorTree.Nodes
{
    [System.Serializable]
    public class GetVec3Node : InputNode
    {
        [OutputType(VariableType.Vector3, "Vector3 Input"), SerializeReference]
        public object input = Vector3.zero;

        public override void Tick(BehaviorController setup)
        {
            ContinueTransitions(this, setup);
        }
    }
}