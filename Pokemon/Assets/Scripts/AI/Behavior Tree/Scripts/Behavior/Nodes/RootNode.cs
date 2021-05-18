using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    [System.Serializable]
    public class RootNode : BaseNode
    {
        public override void Tick(BehaviorController setup)
        {
            ContinueTransitions(this, setup);
        }
    }
}