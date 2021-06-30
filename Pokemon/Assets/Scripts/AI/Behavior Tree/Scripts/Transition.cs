using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes;
using UnityEngine;

namespace Mfknudsen.AI.Behavior_Tree.Scripts
{
    [System.Serializable]
    public class Transition : BaseNode
    {
        public bool transferInformation;
        
        [SerializeReference] public int targetNodeID = -1,
            fromNodeID = -1,
            targetFieldID = -1,
            fromFieldID = -1;

        public Transition(bool transferInformation)
        {
            this.transferInformation = transferInformation;
        }

        public void Set(BaseNode node, int infoID, bool isTarget)
        {
            if(node == null)
                return;
            
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

        public void Set(BaseNode node, bool isTarget)
        {
            if(node == null)
                return;

            if (isTarget)
                targetNodeID = node.id;
            else
                fromNodeID = node.id;
        }

        public override void Tick(BehaviorController setup)
        {
            if (transferInformation)
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
                    foreach (FieldInfo info in targetNode.GetType()
                        .GetFields(BindingFlags.Public | BindingFlags.Instance))
                    {
                        InputType t = info.GetCustomAttribute(typeof(InputType)) as InputType;

                        if (t == null)
                            continue;

                        infos.Add(info);
                    }

                    targetField = infos[targetFieldID];
                    infos.Clear();

                    foreach (FieldInfo info in fromNode.GetType()
                        .GetFields(BindingFlags.Public | BindingFlags.Instance))
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

                if (targetField == null || fromField == null)
                    return;

                object value = fromField.GetValue(fromNode);

                targetField.SetValue(targetNode, value);

                //Set Info Received
                targetNode.AddCheckState(targetField.Name, true);

                if (CheckNodeReady(targetNode))
                    setup.AddNodeToQueue(targetNode);
            }
            else
            {
                BaseNode node = setup.GetNodes()[targetNodeID];

                node.ready = true;
            }
        }

        protected override void Resets()
        {
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