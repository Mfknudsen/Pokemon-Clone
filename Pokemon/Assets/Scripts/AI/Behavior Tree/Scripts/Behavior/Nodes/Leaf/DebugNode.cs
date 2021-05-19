using System;
using AI.BehaviourTreeEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    [System.Serializable]
    public class DebugNode : LeafNode
    {
        [InputType(VariableType.Any, "To Log", true)]
        public object toLog;

        // ReSharper disable Unity.PerformanceAnalysis
        public override void Tick(BehaviorController setup)
        {
            if (toLog == null)
                throw new Exception("Cant Debug Null Object");
            
            Debug.Log("Debug Node Message:\n(" +toLog.GetType()+")"+ toLog);
        }
    }
}