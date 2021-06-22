using System;
using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Leaf
{
    [System.Serializable]
    [Node("Leaf/Debug", "Debug")]
    public class DebugNode : LeafNode
    {
        [InputType(VariableType.Any, "To Log")]
        public object toLog;

        // ReSharper disable Unity.PerformanceAnalysis
        public override void Tick(BehaviorController setup)
        {
            if (toLog == null)
                throw new Exception("Cant Debug Null Object");
            
            Debug.Log("Debug Node Message:\n(" +toLog.GetType()+") : "+ toLog);
        }
    }
}