#region SDK

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes.Input;
using UnityEditor;
using UnityEngine;

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Editor.Custom_Inspectors
{
    [CustomEditor(typeof(BehaviourSetup))]
    public class BehaviourSetupEditor : UnityEditor.Editor
    {
        private BehaviourSetup script;

        private void OnEnable()
        {
            script = (BehaviourSetup) target;
        }

        public override void OnInspectorGUI()
        {
            if (EditorGUILayout.Toggle("RESET", false))
            {
                script.nodes = new List<BaseNode>();
            }

            List<BaseNode> inputs = new List<BaseNode>();
            if (script.nodes == null) return;
            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (BaseNode node in script.nodes)
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                // ReSharper disable once UseNullPropagation
                if (node == null)
                    continue;
                if (node is InputNode && !inputs.Contains(node))
                    inputs.Add(node);
            }

            // ReSharper disable once InvertIf
            if (inputs.Count > 0)
            {
                GUILayout.Space(20);
                GUILayout.Label("Inputs");

                EditorGUILayout.BeginVertical();
                GUIStyle style = GUI.skin.box;
                foreach (BaseNode node in inputs)
                {
                    FieldInfo[] fields = node.GetType()
                        .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                    if (fields.Length == 0)
                        continue;

                    EditorGUILayout.BeginVertical("Box");

                    foreach (FieldInfo info in fields)
                    {
                        OutputType attribute = Attribute.GetCustomAttribute(info, typeof(OutputType)) as OutputType;

                        if (attribute == null)
                            continue;
                        if (attribute.varType == VariableType.DEFAULT)
                            continue;

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(attribute.name,
                            GUILayout.Width(200));

                        object obj = info.GetValue(node);

                        object newValue = EditorMethods.InputField(attribute.varType, obj, attribute.scriptType);

                        if (obj != newValue)
                        {
                            info.SetValue(
                                node,
                                newValue
                            );
                        }

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(10);

            base.OnInspectorGUI();
        }
    }
}