#region SDK

using System.Collections.Generic;
using System.Linq;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input;
using UnityEngine;

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior
{
    [CreateAssetMenu(menuName = "Behaviour Tree/Behaviour", fileName = "New Behaviour", order = 0)]
    public class BehaviourSetup : ScriptableObject
    {
        #region Values
      [SerializeReference]  public List<BaseNode> nodes;

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
            foreach (BaseNode n in nodes.Where(n => n.id == id))
            {
                nodes.Remove(n);
                return;
            }
        }

        public bool AddNode(BaseNode node)
        {
            if (node == null)
                return false;

            if (nodes == null)
                nodes = new List<BaseNode>();

            nodes.Add(node);

            return true;
        }

        #endregion
    }
}