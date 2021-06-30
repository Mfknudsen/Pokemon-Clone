#region SDK

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input;
using UnityEngine;

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Behavior
{
    [CreateAssetMenu(menuName = "Behaviour Tree/Behaviour", fileName = "New Behaviour", order = 0)]
    public class BehaviorSetup : ScriptableObject
    {
        #region Values

        [SerializeReference] public List<BaseNode> nodes;

        #endregion

        public void Setup()
        {
            foreach (BaseNode node in nodes.Where(node => node is Transition))
            {
                Transition transition = (Transition) node;

                if (transition.transferInformation)
                {
                    foreach (BaseNode n in nodes.Where(n =>
                        n.id == transition.fromNodeID && !n.transitions.Contains(transition)))
                        n.transitions.Add(transition);
                }
                else
                {
                    foreach (BaseNode n in nodes.Where(n =>
                        n.id == transition.fromNodeID))
                    {
                        FieldInfo[] fields = n.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

                        int i = 0;

                        foreach (FieldInfo field in fields)
                        {
                            OutCaller outCaller = (OutCaller) field.GetCustomAttribute(typeof(OutCaller));

                            if (outCaller is null)
                                continue;

                            if (transition.fromFieldID != i)
                            {
                                i++;
                                continue;
                            }

                            List<Transition> list = (List<Transition>) field.GetValue(n) ?? new List<Transition>();

                            if (list.Contains(transition)) continue;

                            list.Add(transition);

                            field.SetValue(n, list);
                        }
                    }
                }
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