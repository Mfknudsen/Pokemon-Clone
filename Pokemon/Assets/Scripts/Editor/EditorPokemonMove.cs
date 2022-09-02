﻿#region Packages

using System.Collections.Generic;
using Runtime.Battle.Actions;
using Runtime.Pokémon;
using Runtime.Pokémon.Conditions;
using Runtime.Pokémon.Conditions.Non_Volatiles;
using UnityEditor;
using UnityEngine;

// ReSharper disable ConvertIfStatementToSwitchStatement

#endregion

namespace Editor
{
    [CustomEditor(typeof(PokemonMove))]
    public class EditorPokemonMove : UnityEditor.Editor
    {
        private PokemonMove script;
        private Color standard = new(0.2f, 0.2f, 0.2f);
        private GUIStyle headerStyle, subheaderStyle, textStyle;
        private bool showContest;

        //Assets
        private List<Type> types;
        private string[] typeNames;
        private List<Condition> conditions;
        private string[] conditionNames;

        private void OnEnable()
        {
            //Types
            string[] allTypeAssets = AssetDatabase.FindAssets("t:Type");
            types = new List<Type>();
            typeNames = new string[allTypeAssets.Length + 1];

            types.Add(null);
            typeNames[0] = "Null";
            for (int i = 0; i < allTypeAssets.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(allTypeAssets[i]);
                Type typeAsset = AssetDatabase.LoadAssetAtPath<Type>(assetPath);
                types.Add(typeAsset);
                typeNames[i + 1] = typeAsset.GetTypeName().ToString();
            }

            //Conditions
            string[] allConditionAssets = AssetDatabase.FindAssets("t:Condition");
            conditions = new List<Condition>();
            conditionNames = new string[allConditionAssets.Length + 1];

            conditions.Add(null);
            conditionNames[0] = "Null";
            for (int i = 0; i < allConditionAssets.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(allConditionAssets[i]);
                Condition typeAsset = AssetDatabase.LoadAssetAtPath<Condition>(assetPath);

                if (typeAsset is FaintedCondition)
                    continue;

                conditions.Add(typeAsset);
                conditionNames[i + 1] = typeAsset.GetConditionName();
            }
        }

        public override void OnInspectorGUI()
        {
            standard = GUI.color;
            script = (PokemonMove)target;
            headerStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 30 };
            subheaderStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 25 };
            textStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 20 };

            ShowBattle();

            GUILayout.Space(10);

            ShowTarget();

            GUILayout.Space(10);

            ShowStatus();

            GUILayout.Space(10);

            ShowContests();

            GUILayout.Space(10);

            base.OnInspectorGUI();
            
            EditorUtility.SetDirty(this);
        }

        private void ShowBattle()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Battles", headerStyle);

            //Type
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(150));
            GUILayout.Label("Type", textStyle, GUILayout.Height(30), GUILayout.Width(150));
            EditorGUILayout.EndVertical();

            if (script.GetMoveType() != null)
                GUI.backgroundColor = script.GetMoveType().GetTypeColor();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.backgroundColor = standard;
            int choice = EditorGUILayout.Popup(
                types.IndexOf(script.GetMoveType()),
                typeNames,
                textStyle,
                GUILayout.Height(30)
            );
            script.SetType(types[choice]);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            //Category
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(150));
            GUILayout.Label("Category", textStyle, GUILayout.Height(30), GUILayout.Width(150));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            script.SetCategory((Category)EditorGUILayout.EnumPopup(
                script.GetCategory(),
                textStyle,
                GUILayout.Height(30)
            ));
            script.SetType(types[choice]);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            //PP
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(150));
            GUILayout.Label("PP", textStyle, GUILayout.Height(30), GUILayout.Width(150));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            script.SetStartPP(EditorGUILayout.IntField(
                script.GetStartPP(),
                textStyle,
                GUILayout.Height(30)
            ));
            EditorGUILayout.EndVertical();

            // - Max
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(100));
            GUILayout.Label("Max", textStyle, GUILayout.Height(30), GUILayout.Width(100));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(100));
            script.SetMaxPP(EditorGUILayout.IntField(
                script.GetMaxPP(),
                textStyle,
                GUILayout.Height(30)
            ));
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            //Power
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(150));
            GUILayout.Label("Power", textStyle, GUILayout.Height(30), GUILayout.Width(150));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            script.SetPower(EditorGUILayout.IntField(
                script.GetPower(),
                textStyle,
                GUILayout.Height(30)
            ));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            //Accuracy
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(150));
            GUILayout.Label("Accuracy", textStyle, GUILayout.Height(30), GUILayout.Width(150));
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            script.SetAccuracy(EditorGUILayout.IntField(
                script.GetAccuracy(),
                textStyle,
                GUILayout.Height(30)
            ));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            //Affected
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            string[] affectText = { "Contact", "Protect", "Magic Coat", "Snatch", "Mirror Move", "King's Rock" };
            bool[] truth = script.GetAffected();

            for (int i = 0; i < affectText.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                truth[i] = EditorGUILayout.Toggle(truth[i], GUILayout.Width(30));
                GUILayout.Label(affectText[i]);
                EditorGUILayout.EndHorizontal();
            }

            script.SetAffected(truth);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }

        private void ShowTarget()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Target", headerStyle);

            // ReSharper disable once IdentifierTypo
            bool[] targetable = script.GetTargetable();

            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < 3; i++)
            {
                GUI.backgroundColor = targetable[i] ? Color.green : Color.red;

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUI.backgroundColor = standard;

                GUILayout.FlexibleSpace();
                GUILayout.Label("Foe", subheaderStyle, GUILayout.Width(50));
                targetable[i] = EditorGUILayout.Toggle(
                    targetable[i],
                    GUILayout.Width(15)
                );
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = targetable[5] ? Color.green : Color.red;

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUI.backgroundColor = standard;

            GUILayout.FlexibleSpace();
            GUILayout.Label("Self", subheaderStyle, GUILayout.Width(50));
            targetable[5] = EditorGUILayout.Toggle(
                targetable[5],
                GUILayout.Width(15)
            );
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            for (int i = 3; i < 5; i++)
            {
                GUI.backgroundColor = targetable[i] ? Color.green : Color.red;

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUI.backgroundColor = standard;

                GUILayout.FlexibleSpace();
                GUILayout.Label("Ally", subheaderStyle, GUILayout.Width(50));
                targetable[i] = EditorGUILayout.Toggle(
                    targetable[i],
                    GUILayout.Width(15)
                );
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            script.SetTargetable(targetable);

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Hit Type", textStyle, GUILayout.Height(35));
            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            script.SetHitType((HitType)EditorGUILayout.EnumPopup(
                script.GetHitType(),
                textStyle,
                GUILayout.Height(35)
            ));
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void ShowStatus()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();

            if (EditorGUILayout.Toggle(false, GUILayout.Width(15)))
            {
                script.SetHasStatus(!script.GetHasStatus());
            }

            GUILayout.Label(script.GetHasStatus() ? "Disable Status" : "Enable Status");

            EditorGUILayout.EndHorizontal();

            if (script.GetHasStatus())
            {
                GUILayout.Label("Status", headerStyle);

                //Condition
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(225));
                GUILayout.Label("Condition", textStyle, GUILayout.Height(35), GUILayout.Width(200));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                int choice = EditorGUILayout.Popup(
                    conditions.IndexOf(script.GetStatusCondition()),
                    conditionNames,
                    textStyle,
                    GUILayout.Height(35)
                );

                script.SetStatusCondition(conditions[choice]);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

                //Change
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(225));
                GUILayout.Label("Apply Change", textStyle, GUILayout.Height(35));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                choice = EditorGUILayout.IntField(
                    script.GetApplyChance(),
                    textStyle,
                    GUILayout.Height(35)
                );

                choice = Mathf.Clamp(choice, 0, 100);

                script.SetApplyChance(choice);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        private void ShowContests()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            if (EditorGUILayout.Toggle(false, GUILayout.Width(15)))
            {
                showContest = !showContest;
            }

            GUILayout.Label(showContest ? "Hide Contests" : "Show Contests");

            EditorGUILayout.EndHorizontal();

            if (showContest)
            {
                string[] headers = { "Contests", "Super Contests", "Spectacular Contests" };
                string[] labels = { "Condition", "Appeal", "Jam" };

                for (int j = 0; j < 3; j++)
                {
                    int[] values;

                    if (j == 0)
                        values = script.GetNormalContests();
                    else if (j == 1)
                        values = script.GetSuperContests();
                    else
                        values = script.GetSpectacularContests();

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Label(headers[j], headerStyle);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(200));
                    GUILayout.Label(labels[0], subheaderStyle, GUILayout.Height(40));
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    values[0] = (int)(Contest)EditorGUILayout.EnumPopup(
                        (Contest)values[0],
                        subheaderStyle,
                        GUILayout.Height(40)
                    );
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndHorizontal();

                    for (int i = 1; i < 3; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(200));
                        GUILayout.Label(labels[i], subheaderStyle, GUILayout.Height(40));
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        values[i] = EditorGUILayout.IntField(
                            values[i],
                            subheaderStyle,
                            GUILayout.Height(40)
                        );
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndVertical();

                    if (j == 0)
                        script.SetNormalContests(values);
                    else if (j == 1)
                        script.SetSuperContests(values);
                    else
                        script.SetSpectacularContests(values);
                }
            }

            EditorGUILayout.EndVertical();
        }
    }
}