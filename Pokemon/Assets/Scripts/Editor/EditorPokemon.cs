#region Packages

using System.Collections.Generic;
using Runtime.Battle.Actions;
using Runtime.Pokémon;
using UnityEditor;
using UnityEngine;

#endregion

namespace Editor
{
    [CustomEditor(typeof(Pokemon))]
    public class EditorPokemon : UnityEditor.Editor
    {
        private Pokemon script;
        private Color standard = new(0.2f, 0.2f, 0.2f);
        private GUIStyle headerStyle, subheaderStyle, textStyle;
        private bool showLearnSet;

        //Assets
        private List<PokemonMove> moves;
        private string[] moveNames;
        private List<Type> types;
        private string[] typeNames;
        private List<Pokemon> pokemons;
        private string[] pokemonNames;

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

            //Moves
            string[] allMoveAssets = AssetDatabase.FindAssets("t:PokemonMove");
            moves = new List<PokemonMove>();
            moveNames = new string[allMoveAssets.Length + 1];

            moves.Add(null);
            moveNames[0] = "Null";
            for (int i = 0; i < allMoveAssets.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(allMoveAssets[i]);
                PokemonMove moveAsset = AssetDatabase.LoadAssetAtPath<PokemonMove>(assetPath);
                moves.Add(moveAsset);
                moveNames[i + 1] = moveAsset.GetName();
            }

            //Pokemons
            string[] allPokemonAssets = AssetDatabase.FindAssets("t:Pokemon");
            pokemons = new List<Pokemon>();
            pokemonNames = new string[allPokemonAssets.Length + 1];

            pokemons.Add(null);
            pokemonNames[0] = "Null";
            for (int i = 0; i < allPokemonAssets.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(allPokemonAssets[i]);
                Pokemon pokemonAsset = AssetDatabase.LoadAssetAtPath<Pokemon>(assetPath);
                pokemons.Add(pokemonAsset);
                pokemonNames[i + 1] = pokemonAsset.GetName();
            }
        }

        public override void OnInspectorGUI()
        {
            standard = GUI.color;
            script = (Pokemon)target;
            headerStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 30 };
            subheaderStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 25 };
            textStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 20 };

            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical(GUILayout.Width(200));
            GUILayout.Label("Name", subheaderStyle);
            script.name = GUILayout.TextField(script.GetName(), textStyle);
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginVertical(GUILayout.Width(200));
            GUILayout.Label("Category", subheaderStyle);
            script.SetPokemonCategory(GUILayout.TextField(script.GetPokemonCategory(), textStyle));
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginVertical(GUILayout.Width(200));
            GUILayout.Label("Pokedex", subheaderStyle);
            script.SetPokedexIndex(EditorGUILayout.IntField(script.GetPokedexIndex(), textStyle));
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);
            ShowStats();
            GUILayout.Space(15);
            ShowType();
            GUILayout.Space(15);
            ShowMoves();
            GUILayout.Space(15);
            ShowBreeding();
            GUILayout.Space(15);
            ShowMegaStone();
            GUILayout.Space(15);
            ShowEvYield();

            base.OnInspectorGUI();
        }

        private void ShowStats()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Stats", headerStyle);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            GUILayout.Label("", GUILayout.Height(25), GUILayout.Width(50));
            GUILayout.Label("Base", GUILayout.Height(25), GUILayout.Width(50));
            GUILayout.Label("IV", GUILayout.Height(25), GUILayout.Width(50));
            GUILayout.Label("EV", GUILayout.Height(25), GUILayout.Width(50));
            EditorGUILayout.EndVertical();

            int w = 90, h = 25;

            int statTotal = 0, ivTotal = 0, evTotal = 0;
            for (int i = 0; i < 6; i++)
            {
                EditorGUILayout.BeginVertical();
                GUILayout.Label(((Stat)i).ToString(), textStyle, GUILayout.Height(h), GUILayout.Width(w));
                //Stat
                script.SetStat((Stat)i,
                    EditorGUILayout.IntField(script.GetStatRaw((Stat)i), GUILayout.Height(h), GUILayout.Width(w)));
                statTotal += script.GetStatRaw((Stat)i);
                //IV
                script.SetIV((Stat)i,
                    EditorGUILayout.IntField(script.GetIV((Stat)i), GUILayout.Height(h), GUILayout.Width(w)));
                ivTotal += script.GetIV((Stat)i);
                //EV
                script.SetEV((Stat)i,
                    EditorGUILayout.IntField(script.GetEV((Stat)i), GUILayout.Height(h), GUILayout.Width(w)));
                evTotal = script.GetEV((Stat)i);
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginVertical();
            GUILayout.Label("Total", subheaderStyle, GUILayout.Height(h), GUILayout.Width(w));
            GUILayout.Label("" + statTotal, GUILayout.Height(h), GUILayout.Width(w));
            GUILayout.Label("" + ivTotal, GUILayout.Height(h), GUILayout.Width(w));
            GUILayout.Label("" + evTotal, GUILayout.Height(h), GUILayout.Width(w));
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void ShowType()
        {
            Type[] toDisplay = script.GetTypes();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Type:", headerStyle);

            EditorGUILayout.BeginHorizontal();

            if (toDisplay[0] != null)
                GUI.backgroundColor = toDisplay[0].GetTypeColor();
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUI.backgroundColor = standard;

            int choice = EditorGUILayout.Popup(
                types.IndexOf(toDisplay[0]),
                typeNames,
                textStyle,
                GUILayout.Height(50));

            toDisplay[0] = types[choice];
            EditorGUILayout.EndHorizontal();

            if (toDisplay[1] != null)
            {
                GUI.backgroundColor = toDisplay[1].GetTypeColor();
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUI.backgroundColor = standard;

                choice = EditorGUILayout.Popup(
                    types.IndexOf(toDisplay[1]),
                    typeNames,
                    textStyle,
                    GUILayout.Height(50));

                toDisplay[1] = types[choice];
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndHorizontal();

            if (toDisplay[1] == null)
            {
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(50));
                GUILayout.Label("Add ");
                if (EditorGUILayout.Toggle(false))
                    toDisplay = new[] { toDisplay[0], types[Random.Range(1, toDisplay.Length - 1)] };
                EditorGUILayout.EndHorizontal();
            }

            script.SetTypes(toDisplay);

            EditorGUILayout.EndVertical();
        }

        private void ShowBreeding()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Breeding", headerStyle);
            EditorGUILayout.BeginHorizontal();

            //Egg
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(80));
            GUILayout.Label("Egg Group", subheaderStyle);
            GUILayout.FlexibleSpace();
            script.SetEggGroup((EggGroup)EditorGUILayout.EnumPopup(script.GetEggGroup(), textStyle));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();

            //Hatch Time
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(80));
            GUILayout.Label("Hatch Time", subheaderStyle);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Min: ", GUILayout.Width(45));
            script.SetMinHatchSteps(EditorGUILayout.IntField(script.GetMinHatchSteps(), GUILayout.Width(100)));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Max: ", GUILayout.Width(45));
            script.SetMaxHatchSteps(EditorGUILayout.IntField(script.GetMaxHatchSteps(), GUILayout.Width(100)));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            //Height / Weight
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(80));
            GUILayout.Label("Height / Weight", subheaderStyle);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Meter: ", GUILayout.Width(45));
            script.SetHeight(EditorGUILayout.FloatField(script.GetHeight(), GUILayout.Width(100)));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Kg: ", GUILayout.Width(45));
            script.SetWeight(EditorGUILayout.FloatField(script.GetWeight(), GUILayout.Width(100)));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(15);
            EditorGUILayout.BeginHorizontal();

            //
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(90));
            GUILayout.Label("Gender Ratio", subheaderStyle);
            script.SetGenderRate(EditorGUILayout.FloatField(
                script.GetGenderRate(),
                textStyle
            ));
            GUILayout.Label("Male: " + script.GetGenderRate() + "% | Female: " + (100 - script.GetGenderRate()) + "%",
                textStyle);
            EditorGUILayout.EndVertical();
            //
            //
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(90));
            GUILayout.Label("Catch Rate", subheaderStyle);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("%", textStyle);
            script.SetCatchRate(Mathf.Clamp(EditorGUILayout.FloatField(
                script.GetCatchRate(),
                textStyle
            ), 0, 100));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            //

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void ShowMoves()
        {
            PokemonMove[] toDisplay = script.GetMoves();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Moves", headerStyle);

            #region Learned

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Learned Moves", subheaderStyle);
            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < 4; i++)
            {
                if (toDisplay[i] is null) continue;

                GUI.backgroundColor = toDisplay[i].GetMoveType().GetTypeColor();
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUI.backgroundColor = standard;

                int choice = EditorGUILayout.Popup(
                    moves.IndexOf(toDisplay[i]),
                    moveNames,
                    textStyle,
                    GUILayout.Height(50));
                EditorGUILayout.EndVertical();


                script.SetLearnedMove(i, moves[choice]);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (toDisplay[0] is null || toDisplay[1] is null || toDisplay[2] is null || toDisplay[3] is null)
            {
                EditorGUILayout.BeginVertical(GUILayout.MaxWidth(50));
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Add ");
                if (EditorGUILayout.Toggle(false, GUILayout.Width(20)))
                {
                    int i;

                    if (toDisplay[0] == null)
                        i = 0;
                    else if (toDisplay[1] == null)
                        i = 1;
                    else if (toDisplay[2] == null)
                        i = 2;
                    else
                        i = 3;

                    toDisplay[i] = moves[Random.Range(1, moves.Count - 1)];
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }

            #endregion

            #region Learnset

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(showLearnSet ? "Hide Learn Set" : "Show Learn Set", GUILayout.Width(90));

            if (EditorGUILayout.Toggle(false))
                showLearnSet = !showLearnSet;

            EditorGUILayout.EndHorizontal();
            if (showLearnSet)
            {
                #region By Level

                GUILayout.Label("By Level", subheaderStyle);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Level", textStyle);
                GUILayout.Label("Move", textStyle);
                EditorGUILayout.EndHorizontal();

                int[] intKeys = script.GetLevelLearnedMovesKeys();
                PokemonMove[] values = script.GetLevelLearnableMoveValue();

                List<int> resultKeys = new();
                List<PokemonMove> resultValue = new();

                for (int i = 0; i < intKeys.Length; i++)
                {
                    int intKey = intKeys[i];
                    PokemonMove value = values[i];

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(30));
                    intKey = Mathf.Clamp(EditorGUILayout.IntField(intKey,
                        textStyle,
                        GUILayout.Height(25)), 0, 100);
                    EditorGUILayout.EndVertical();

                    GUI.backgroundColor = value.GetMoveType().GetTypeColor();
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(30));
                    GUI.backgroundColor = standard;
                    GUILayout.FlexibleSpace();
                    int choice = EditorGUILayout.Popup(
                        moves.IndexOf(value),
                        moveNames,
                        textStyle,
                        GUILayout.Height(25));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndHorizontal();

                    value = moves[choice];

                    if (value == null) continue;
                    
                    resultKeys.Add(intKey);
                    resultValue.Add(value);
                }

                if (EditorGUILayout.Toggle(false))
                {
                    resultKeys.Add(0);
                    resultValue.Add(moves[Random.Range(1, moves.Count - 1)]);
                }

                script.SetLevelLearnedMoveKeys(resultKeys.ToArray());
                script.SetLevelLearnableMoveValues(resultValue.ToArray());
                EditorGUILayout.EndVertical();

                #endregion

                GUILayout.Space(10);

                #region By TM/TR

                GUILayout.Label("By TM/TR", subheaderStyle);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                resultValue = new List<PokemonMove>();
                values = script.GetTMLearnableMoveValues();

                foreach (PokemonMove v in values)
                {
                    PokemonMove value = v;
                    
                    GUI.backgroundColor = value.GetMoveType().GetTypeColor();
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(30));
                    GUI.backgroundColor = standard;

                    int choice = EditorGUILayout.Popup(
                        moves.IndexOf(value),
                        moveNames,
                        textStyle,
                        GUILayout.Height(25));
                    EditorGUILayout.EndVertical();

                    value = moves[choice];

                    if (value != null)
                        resultValue.Add(value);
                }

                if (EditorGUILayout.Toggle(false))
                    resultValue.Add(moves[Random.Range(1, moves.Count - 1)]);

                script.SetTMLearnableMoveValues(resultValue.ToArray());
                EditorGUILayout.EndVertical();

                #endregion

                GUILayout.Space(10);

                #region By Breeding

                GUILayout.Label("By Breeding", subheaderStyle);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Pokemon", textStyle);
                GUILayout.Label("Move", textStyle);
                EditorGUILayout.EndHorizontal();

                Pokemon[] pokemonKeys = script.GetBreedingLearnableMoveKeys();
                values = script.GetBreedingLearnableMoveValue();

                List<Pokemon> resultValuePoke = new();
                resultValue = new List<PokemonMove>();

                for (int i = 0; i < pokemonKeys.Length; i++)
                {
                    Pokemon pokemonKey = pokemonKeys[i];
                    PokemonMove value = values[i];

                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(30));
                    pokemonKey = pokemons[EditorGUILayout.Popup(
                        pokemons.IndexOf(pokemonKey),
                        pokemonNames,
                        textStyle,
                        GUILayout.Height(25))];
                    EditorGUILayout.EndVertical();

                    GUI.backgroundColor = value.GetMoveType().GetTypeColor();
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(30));
                    GUI.backgroundColor = standard;
                    GUILayout.FlexibleSpace();
                    int choice = EditorGUILayout.Popup(
                        moves.IndexOf(value),
                        moveNames,
                        textStyle,
                        GUILayout.Height(25));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.EndHorizontal();

                    value = moves[choice];

                    if (value != null && pokemonKey != null)
                    {
                        resultValuePoke.Add(pokemonKey);
                        resultValue.Add(value);
                    }
                }

                if (EditorGUILayout.Toggle(false))
                {
                    resultValuePoke.Add(pokemons[Random.Range(1, pokemons.Count - 1)]);
                    resultValue.Add(moves[Random.Range(1, moves.Count - 1)]);
                }

                script.SetBreedingLearnableMoveKeys(resultValuePoke.ToArray());
                script.SetBreedingLearnableMoveValues(resultValue.ToArray());

                EditorGUILayout.EndVertical();

                #endregion

                GUILayout.Space(10);

                #region By Tutor

                GUILayout.Label("By Tutor", subheaderStyle);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                resultValue = new List<PokemonMove>();
                values = script.GetTutorLearnableMoveValue();

                foreach (PokemonMove v in values)
                {
                    PokemonMove value = v;
                    GUI.backgroundColor = value.GetMoveType().GetTypeColor();
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Height(30));
                    GUI.backgroundColor = standard;

                    int choice = EditorGUILayout.Popup(
                        moves.IndexOf(value),
                        moveNames,
                        textStyle,
                        GUILayout.Height(25));
                    EditorGUILayout.EndVertical();

                    value = moves[choice];

                    if (value != null)
                        resultValue.Add(value);
                }

                if (EditorGUILayout.Toggle(false))
                    resultValue.Add(moves[Random.Range(1, moves.Count - 1)]);

                script.SetTutorLearnableMoveValue(resultValue.ToArray());
                EditorGUILayout.EndVertical();

                #endregion
            }

            EditorGUILayout.EndVertical();

            #endregion
        }

        private void ShowMegaStone()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);


            EditorGUILayout.BeginHorizontal();
            //
            EditorGUILayout.BeginVertical();
            GUILayout.Label("Base experience yield", headerStyle);

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.FlexibleSpace();
            script.SetExpYield(EditorGUILayout.IntField(
                script.GetExpYield(),
                textStyle,
                GUILayout.Height(40)));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            //

            //
            EditorGUILayout.BeginVertical();
            GUILayout.Label("Leveling rate", headerStyle);

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            GUILayout.FlexibleSpace();
            script.SetLevelingRate((LevelRate)EditorGUILayout.EnumPopup(
                script.GetLevelRate(),
                textStyle,
                GUILayout.Height(40)));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            //
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void ShowEvYield()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("EV Yield", headerStyle);

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

            int w = 90, h = 25;

            for (int i = 0; i < 6; i++)
            {
                EditorGUILayout.BeginVertical();
                GUILayout.Label(((Stat)i).ToString(), textStyle, GUILayout.Height(h), GUILayout.Width(w));
                script.SetEVYield((Stat)i, EditorGUILayout.IntField(script.GetEVYield((Stat)i)));
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
    }
}