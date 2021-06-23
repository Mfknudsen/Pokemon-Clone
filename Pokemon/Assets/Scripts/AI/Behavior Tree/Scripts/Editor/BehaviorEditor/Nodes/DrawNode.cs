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
        public static int DisplayCalls(BaseNodeSetting b, BaseNode node)
        {
            if (b == null || node == null) return 0;

            GUIStyle style = new GUIStyle();
            int i = 1, height = (int) b.windowRect.height;
            float xLeftOffset = b.windowRect.x + 7.5f,
                xRightOffset = b.windowRect.x + b.windowRect.width - 7.5f,
                yStart = b.windowRect.y + b.windowRect.height;

            EditorGUILayout.BeginHorizontal();

            FieldInfo[] fields = node.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

            EditorGUILayout.BeginVertical(GUILayout.Width(b.windowRect.width / 2));

            if (node.inCall)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("", GUILayout.Width(20)))
                {
                    //Only activate once
                    Debug.Log(new Vector2(xLeftOffset, yStart + 40 - 12.5f));
                    BehaviorEditor.editor.MakeActionTransition(b, true, new Vector2(xLeftOffset, yStart + 40 - 12.5f));
                    Debug.Log("Done");
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical();

            foreach (FieldInfo f in fields)
            {
                OutCaller c = Attribute.GetCustomAttribute(f, typeof(OutCaller)) as OutCaller;

                if (c == null) continue;

                height += 40;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(c.display,
                    GUILayout.MaxWidth(style.CalcSize(new GUIContent(c.display)).x + 2));

                if (GUILayout.Button("", GUILayout.Width(20)))
                {
                    Debug.Log(new Vector2(xRightOffset, yStart + i * 40 - 12.5f));
                    BehaviorEditor.editor.MakeActionTransition(b, false,
                        new Vector2(xRightOffset, yStart + i * 40 - 12.5f));
                    Debug.Log("Done");
                }

                i++;

                EditorGUILayout.EndHorizontal();
            }

            b.windowRect.height = height;

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            if (node.inCall)
                i = (int) Mathf.Clamp(i, 1, Mathf.Infinity);

            return i;
        }

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
                if (attribute.varType == VariableType.Default)
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
                if (attribute.varType == VariableType.Default)
                    continue;

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField(attribute.name);
                EditorGUILayout.BeginHorizontal();

                //
                i++;
                if (EditorGUILayout.Toggle(false, GUILayout.Width(15)))
                {
                    BehaviorEditor.editor.MakeInformationTransition(
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
                if (attribute.varType == VariableType.Default)
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
                if (attribute.varType == VariableType.Default)
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
                    BehaviorEditor.editor.MakeInformationTransition(
                        b,
                        (i - 1),
                        (int) attribute.varType,
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