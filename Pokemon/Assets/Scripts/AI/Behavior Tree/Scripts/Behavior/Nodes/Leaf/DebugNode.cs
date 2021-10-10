using System;
using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Leaf
{
    [Serializable]
    [Node("Leaf/Debug", "Debug")]
    public class DebugNode : LeafNode
    {
        [InputType("To Log", null)]
        public object toLog;

        // ReSharper disable Unity.PerformanceAnalysis
        public override void Tick(BattleAI setup)
        {
            if (toLog == null)
                throw new Exception("Cant Debug Null Object");
            
            Debug.Log("Debug Node Message:\n(" +toLog.GetType()+") : "+ toLog);
            
            Resets();
        }

        protected override void Resets()
        {
            toLog = null;
        }
    }
}