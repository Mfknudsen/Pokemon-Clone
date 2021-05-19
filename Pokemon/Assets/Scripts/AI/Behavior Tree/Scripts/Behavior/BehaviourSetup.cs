#region SDK

using System.Collections.Generic;
using System.Linq;
using AI.BehaviorTree.Nodes;
using UnityEngine;

#endregion

namespace AI.BehaviorTree
{
    [CreateAssetMenu(menuName = "Behaviour Tree/Behaviour", fileName = "New Behaviour", order = 0)]
    public class BehaviourSetup : ScriptableObject
    {
        #region Values

        [SerializeReference] public List<BaseNode> nodes;

        #endregion

        public void Setup()
        {
            foreach (BaseNode node in nodes.Where(node => node is Transition))
            {
                Transition transition = (Transition) node;

                foreach (BaseNode n in nodes.Where(n =>
                    n.id == transition.fromNodeID && !n.transitions.Contains(transition)))
                    n.transitions.Add(transition);
            }
        }

        public void Tick(BehaviorController setup)
        {
            foreach (BaseNode node in nodes.Where(node => node is InputNode))
                node.Tick(setup);

            foreach (BaseNode node in nodes.Where(node => node is RootNode))
                node.Tick(setup);
        }

        #region Getters

        public BaseNode GetNodeByID(int id)
        {
            if (nodes == null)
                return null;
            foreach (BaseNode node in nodes)
            {
                if (node != null)
                    if (node.id == id)
                        return node;
            }

            return null;
        }

        #endregion

        #region In

        public void RemoveNode(int id)
        {
            BaseNode n = nodes.FirstOrDefault(node => node.id == id);

            nodes.Remove(n);
        }

        public bool AddNode(BaseNode node)
        {
            if (node == null)
                return false;

            if (nodes == null)
                nodes = new List<BaseNode>();

            if (HasRoot() && node is RootNode)
                return false;

            nodes.Add(node);

            return true;
        }

        #endregion

        #region Out

        public bool HasRoot()
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (BaseNode n in nodes.Where(n => n is RootNode))
                return true;
            return false;
        }

        #endregion
    }
}