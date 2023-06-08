#region Libraries

using Runtime.Pokémon;
using UnityEditor;

#endregion

public class Pokedex
{
    #region Values

    private static Pokedex instance;

    private static readonly string ScriptableObjectAssetFolder = "Assets/ScriptableObjects/Pokédex/";

    #endregion

    #region Getters

    private static Pokedex GetInstance()
    {
        instance ??= new Pokedex();

        return instance;
    }

    #endregion

    #region Out

    public static Pokemon Get(string path)
    {
        Pokemon result = AssetDatabase.LoadAssetAtPath<Pokemon>(ScriptableObjectAssetFolder + path);

        return result;
    }

    #endregion
}