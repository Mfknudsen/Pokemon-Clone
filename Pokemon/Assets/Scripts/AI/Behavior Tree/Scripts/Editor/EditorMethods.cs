#region SDK

using UnityEditor;
using UnityEngine; //Custom

#endregion

namespace Mfknudsen.AI.Behavior_Tree.Scripts.Editor
{
    public class EditorMethods
    {
        public static object InputField(VariableType type, object input, ScriptType sType)
        {
            try
            {
                switch (type)
                {
                    case VariableType.Int:
                        return EditorGUILayout.IntField((int) input, GUILayout.MaxWidth(185));
                    case VariableType.Float:
                        return EditorGUILayout.FloatField((float) input, GUILayout.MaxWidth(185));
                    case VariableType.Double:
                        return EditorGUILayout.DoubleField((double) input, GUILayout.MaxWidth(185));
                    case VariableType.String:
                        return EditorGUILayout.TextField((string) input, GUILayout.MaxWidth(185));
                    case VariableType.Vector2:
                        return EditorGUILayout.Vector2Field("", (Vector2) input, GUILayout.MaxWidth(185));
                    case VariableType.Vector3:
                        return EditorGUILayout.Vector3Field("", (Vector3) input, GUILayout.MaxWidth(185));
                    case VariableType.Transform:
                        return EditorGUILayout.ObjectField("", (Transform) input, typeof(Transform), true,
                                GUILayout.MaxWidth(185))
                            as Transform;
                    case VariableType.Script:
                        return ScriptField(input, sType);

                    case VariableType.Any:
                        EditorGUILayout.LabelField("Any Accepted");
                        break;
                
                    case VariableType.DEFAULT:
                        EditorGUILayout.LabelField("SET VARIABLE-TYPE OF FIELD ATTRIBUTE");
                        break;
                    default:
                        EditorGUILayout.LabelField("UNKNOWN VARIABLE TYPE");
                        break;
                }
            }
            catch
            {
                // ignored
            }

            return null;
        }

        private static object ScriptField(object input, ScriptType type)
        {
            if (type == ScriptType.DEFAULT)
                return null;
            try
            {
                switch (type)
                {
                    case ScriptType.Pokemon:
                        return EditorGUILayout.ObjectField("", (Monster.Pokemon) input, typeof(Monster.Pokemon), true,
                            GUILayout.MaxWidth(185));
                    case ScriptType.PokeMove:
                        return EditorGUILayout.ObjectField("", (Battle.Actions.Moves.PokemonMove) input,
                            typeof(Battle.Actions.Moves.PokemonMove), true, GUILayout.MaxWidth(185));
                    case ScriptType.TrainerTeam:
                        return EditorGUILayout.ObjectField("", (Trainer.Team) input, typeof(Trainer.Team), true,
                            GUILayout.MaxWidth(185));

                    // ReSharper disable once UnreachableSwitchCaseDueToIntegerAnalysis
                    case ScriptType.DEFAULT:
                        EditorGUILayout.LabelField("SET SCRIPT-TYPE OF FIELD ATTRIBUTE");
                        break;
                    default:
                        EditorGUILayout.LabelField("UKNOWN SCRIPT TYPE");
                        break;
                }
            }
            catch
            {
                // ignored
            }

            return null;
        }
    }
}