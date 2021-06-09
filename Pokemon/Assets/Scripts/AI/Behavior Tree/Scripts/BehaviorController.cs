using System.Collections;
using System.Collections.Generic;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes;
using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts
{
    public class BehaviorController : MonoBehaviour
    {
        public BehaviourSetup behaviour;
        private IEnumerator threadInstance = null;
        private List<BaseNode> nodeQueue = new List<BaseNode>();

        private void Start()
        {
            behaviour.Setup();
        }

        private void Update()
        {
            if (nodeQueue.Count == 0 && threadInstance == null)
                behaviour.Tick(this);

            if (nodeQueue.Count > 0)
            {
                nodeQueue[0].Tick(this);
                nodeQueue.RemoveAt(0);
            }
        }

        public void AddNodeToQueue(BaseNode node)
        {
            if (node == null)
                return;

            if (!nodeQueue.Contains(node))
                nodeQueue.Add(node);
        }

        public BaseNode[] GetNodes()
        {
            return behaviour.nodes.ToArray();
        }
    }
}