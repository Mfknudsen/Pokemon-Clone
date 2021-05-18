using System;
using System.Collections;
using System.Collections.Generic;
using AI.BehaviorTree.Nodes;
using UnityEngine;

namespace AI.BehaviorTree
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
                Debug.Log(nodeQueue[0].id);
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