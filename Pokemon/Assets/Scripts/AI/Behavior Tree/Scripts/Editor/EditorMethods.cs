#region SDK

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//Custom
using AI.BehaviourTreeEditor;

#endregion

public class EditorMethods
{
    public static object InputField(VariableType type, object input)
    {
        try
        {
            if (type == VariableType.Int)
                return EditorGUILayout.IntField((int) input);
            else if (type == VariableType.Float)
                return EditorGUILayout.FloatField((float) input);
            else if (type == VariableType.String)
                return EditorGUILayout.TextField((string) input);
            else if (type == VariableType.Vector2)
                return EditorGUILayout.Vector2Field("", (Vector2) input, GUILayout.MaxWidth(185));
            else if (type == VariableType.Vector3)
                return EditorGUILayout.Vector3Field("", (Vector3) input, GUILayout.MaxWidth(185));
            else if (type == VariableType.Quaterion)
                return null; //No inputfield
            else if (type == VariableType.Transform)
                return EditorGUILayout.ObjectField("", (Transform) input, typeof(Transform), true,
                        GUILayout.MaxWidth(185))
                    as Transform;
        }
        catch
        {
        }

        return null;
    }
}