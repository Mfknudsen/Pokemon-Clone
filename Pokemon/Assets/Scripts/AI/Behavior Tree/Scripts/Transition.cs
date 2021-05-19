using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI.Behavior_Tree.Scripts.Behavior.Nodes.Filler.Math;
using UnityEngine;
using AI.BehaviourTreeEditor;

namespace AI.BehaviorTree.Nodes
{
    [System.Serializable]
    public class Transition : BaseNode
    {
        [SerializeReference] public int targetNodeID = -1,
            fromNodeID = -1,
            targetFieldID = -1,
            fromFieldID = -1;

        public void Set(BaseNode node, int infoID, bool isTarget)
        {
            if (isTarget)
            {
                targetFieldID = infoID;
                targetNodeID = node.id;
            }
            else
            {
                fromFieldID = infoID;
                fromNodeID = node.id;
            }
        }

        public override void Tick(BehaviorController setup)
        {
            BaseNode targetNode = null, fromNode = null;
            FieldInfo targetField, fromField;

            //Get the Nodes
            foreach (BaseNode n in setup.GetNodes().Where(n => n.id == targetNodeID))
                targetNode = n;
            foreach (BaseNode n in setup.GetNodes().Where(n => n.id == fromNodeID))
                fromNode = n;
            //Get the FieldInfos
            try
            {
                List<FieldInfo> infos = new List<FieldInfo>();
                foreach (FieldInfo info in targetNode.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    InputType t = info.GetCustomAttribute(typeof(InputType)) as InputType;

                    if (t == null)
                        continue;

                    infos.Add(info);
                }

                targetField = infos[targetFieldID];
                infos.Clear();

                foreach (FieldInfo info in fromNode.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
                {
                    OutputType t = info.GetCustomAttribute(typeof(OutputType)) as OutputType;

                    if (t == null)
                        continue;

                    infos.Add(info);
                }

                fromField = infos[fromFieldID];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }

            if (targetNode == null || fromNode == null || targetField == null || fromField == null)
                return;

            object value = fromField.GetValue(fromNode);

            value = ReturnAsType(value, ((InputType) targetField.GetCustomAttribute(typeof(InputType))).varType);
            targetField.SetValue(targetNode, value);
            
            //Set Info Received
            targetNode.AddCheckState(targetField.Name, true);

            if (CheckNodeReady(targetNode))
                setup.AddNodeToQueue(targetNode);
        }

        private object ReturnAsType(object o, VariableType type)
        {
            if (o == null)
                return null;
            
            String s = o.ToString();
            if (type == VariableType.Float)
                return float.Parse(s);

            return o;
        }
    }
}