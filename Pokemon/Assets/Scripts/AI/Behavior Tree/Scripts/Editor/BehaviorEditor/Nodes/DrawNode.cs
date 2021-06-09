#region SDK

using System;
using System.Collections.Generic;
using System.Reflection;
using Mfknudsen.AI.Behavior_Tree.Scripts.Behavior.Nodes;
using UnityEditor;
using UnityEngine; //Custom

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Editor.BehaviorEditor.Nodes
{
    public abstract class DrawNode : ScriptableObject
    {
        public abstract void DrawWindow(BaseNodeSetting b, BaseNode node);

        public abstract void DrawCurve(BaseNodeSetting b, BaseNode node);
    }

    public static class NodeFunc
    {
        public static int DisplayInputs(BaseNodeSetting b, BaseNode node, int extra)
        {
            if (node == null)
                return 0;

            int i = 0;

            FieldInfo[] fields = node.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            List<FieldInfo> input = new List<FieldInfo>();
            int height = (int) b.windowRect.height;
            foreach (FieldInfo f in fields)
            {
                InputType attribute = Attribute.GetCustomAttribute(f, typeof(InputType)) as InputType;

                if (attribute == null)
                    continue;
                if (attribute.varType == VariableType.DEFAULT)
                    continue;

                height += 40;

                input.Add(f);
            }

            b.windowRect.height = height + 7.5f;

            EditorGUILayout.BeginVertical("box");
            //All outputs
            foreach (FieldInfo field in input)
            {
                InputType attribute = Attribute.GetCustomAttribute(field, typeof(InputType)) as InputType;

                if (attribute == null)
                    continue;
                if (attribute.varType == VariableType.DEFAULT)
                    continue;

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(attribute.name);
                EditorGUILayout.BeginHorizontal();

                //
                i++;
                if (EditorGUILayout.Toggle(false, GUILayout.Width(15)))
                {
                    BehaviorEditor.editor.MakeTransition(
                        b,
                        (-1 + i),
                        (int) attribute.varType,
                        b.windowRect.position + new Vector2(7.5f, 25 + ((i + extra) * 40) - 12.5f),
                        true
                    );
                }

                object obj = field.GetValue(node);
                field.SetValue(
                    node,
                    EditorMethods.InputField(
                        attribute.varType,
                        obj,
                        attribute.scriptType)
                );

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();

            return i;
        }

        public static int DisplayOutputs(BaseNodeSetting b, BaseNode node, int extra)
        {
            if (node == null)
                return 0;

            int i = 0;

            FieldInfo[] fields = node.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            List<FieldInfo> output = new List<FieldInfo>();
            int height = (int) b.windowRect.height;
            foreach (FieldInfo f in fields)
            {
                OutputType attribute = Attribute.GetCustomAttribute(f, typeof(OutputType)) as OutputType;

                if (attribute == null)
                    continue;
                if (attribute.varType == VariableType.DEFAULT)
                    continue;

                height += 40;

                output.Add(f);
            }

            b.windowRect.height = height + 7.5f;

            EditorGUILayout.BeginVertical("box");
            //All outputs
            foreach (FieldInfo field in output)
            {
                OutputType attribute = Attribute.GetCustomAttribute(field, typeof(OutputType)) as OutputType;

                if (attribute == null)
                    continue;
                if (attribute.varType == VariableType.DEFAULT)
                    continue;

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(attribute.name);

                EditorGUILayout.BeginHorizontal();

                object obj = field.GetValue(node);
                field.SetValue(
                    node,
                    EditorMethods.InputField(
                        attribute.varType,
                        obj,
                        attribute.scriptType)
                );

                //
                i++;
                if (EditorGUILayout.Toggle(false, GUILayout.Width(15)))
                {
                    BehaviorEditor.editor.MakeTransition(
                        b,
                        (i - 1),
                        (int)attribute.varType,
                        b.windowRect.position +
                        new Vector2(b.windowRect.width - 7.5f, 25 + 10 + ((i + extra) * 40) - 12.5f),
                        false
                    );
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();

            return i;
        }
    }
}