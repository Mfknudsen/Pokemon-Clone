using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AI.BehaviourTreeEditor;
using UnityEngine;

namespace AI.BehaviorTree.Nodes
{
    [System.Serializable]
    public abstract class BaseNode : object
    {
        public int id;

        public List<Transition> transitions;
        public Dictionary<string, bool> checkState;

        public abstract void Tick(BehaviorController setup);

        public void Reset(BaseNode n)
        {
            foreach (FieldInfo info in n.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (info.GetCustomAttribute(typeof(InputType)) is InputType input)
                    ResetField(n, info, input.varType);
                //else if (info.GetCustomAttribute(typeof(OutputType)) is OutputType output)
                    //ResetField(n, info, output.varType);
            }

            n.checkState = new Dictionary<string, bool>();
        }

        protected void ContinueTransitions(BaseNode n, BehaviorController setup)
        {
            if (transitions == null) return;
            foreach (Transition t in transitions)
                t.Tick(setup);

            Reset(n);
        }

        public void AddTransition(Transition transition)
        {
            if (transitions == null)
                transitions = new List<Transition>();

            transitions.Add(transition);
        }

        public void AddCheckState(string key, bool value)
        {
            if (checkState == null)
                checkState = new Dictionary<string, bool>();

            if (!checkState.ContainsKey(key))
                checkState.Add(key, value);
        }

        protected bool CheckNodeReady(BaseNode n)
        {
            FieldInfo[] infos = n.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);

            int i = 0;
            foreach (FieldInfo f in infos.Where(f => f.GetCustomAttribute(typeof(InputType)) as InputType != null))
            {
                i++;

                foreach (string c in n.checkState.Keys)
                {
                    if (!f.Name.Equals(c)) continue;

                    if (!n.checkState[c])
                        return false;
                }
            }

            return i == n.checkState.Keys.Count;
        }

        protected void ResetField(BaseNode n, FieldInfo info, VariableType varType)
        {
            if (varType == VariableType.Float)
                info.SetValue(n, 0.0f);
            else if (varType == VariableType.Int)
                info.SetValue(n, 0);
            else if (varType == VariableType.String)
                info.SetValue(n, "");
            else if (varType == VariableType.Vector2)
                info.SetValue(n, Vector2.zero);
            else if (varType == VariableType.Vector3)
                info.SetValue(n, Vector3.zero);
            else if (varType == VariableType.Quaterion)
                info.SetValue(n, Quaternion.identity);
            else if (varType == VariableType.Transform)
                info.SetValue(n, null);
        }
    }
}