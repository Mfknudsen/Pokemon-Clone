#region SDK

using System.Reflection;
using Mfknudsen.Battle.Actions.Move;
using Mfknudsen.Pokémon;
using Mfknudsen.Trainer;
using UnityEditor;
using UnityEngine;
using Type = System.Type;

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Editor
{
    public static class EditorMethods
    {
        public static object InputField(Type type, object input)
        {
            //Standard
            if (type == typeof(bool))
                return EditorGUILayout.Toggle((bool) input);
            if (type == typeof(int))
                return EditorGUILayout.IntField((int) input);
            if (type == typeof(float))
                return EditorGUILayout.FloatField((float) input);
            if (type == typeof(double))
                return EditorGUILayout.DoubleField((double) input);
            if (type == typeof(string))
                return EditorGUILayout.TextField((string) input);
            if (type == typeof(Vector2))
                return EditorGUILayout.Vector2Field("", (Vector2) input, GUILayout.MaxWidth(185));
            if (type == typeof(Vector3))
                return EditorGUILayout.Vector3Field("", (Vector3) input, GUILayout.MaxWidth(185));
            if (type == typeof(Transform))
                return EditorGUILayout.ObjectField((Transform) input, typeof(Transform), true);
            if (type == typeof(GameObject))
                return EditorGUILayout.ObjectField((GameObject) input, typeof(GameObject), true);
            //Pokemon
            if (type == typeof(Pokemon))
                return EditorGUILayout.ObjectField((Pokemon) input, typeof(Pokemon), true);
            if (type == typeof(PokemonMove))
                return EditorGUILayout.ObjectField((PokemonMove) input, typeof(PokemonMove), true);
            if (type == typeof(Team))
                return EditorGUILayout.ObjectField((Team) input, typeof(Team), true);

            return null;
        }
    }
}