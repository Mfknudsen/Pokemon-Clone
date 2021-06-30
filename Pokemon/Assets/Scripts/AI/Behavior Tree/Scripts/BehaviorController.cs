#region SDK

using System.Collections.Generic;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes;
using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts
{
    public class BehaviorController : MonoBehaviour
    {
        #region Values

        [FormerlySerializedAs("behaviour")] public BehaviorSetup behavior;
        private readonly List<BaseNode> nodeQueue = new List<BaseNode>();

        #endregion
        
        public void AddNodeToQueue(BaseNode node)
        {
            if (node == null)
                return;

            if (!nodeQueue.Contains(node))
                nodeQueue.Add(node);
        }

        public BaseNode[] GetNodes()
        {
            return behavior.nodes.ToArray();
        }

        public void TickBrain()
        {
            behavior.Setup();
            behavior.Tick(this);
        }
    }
}