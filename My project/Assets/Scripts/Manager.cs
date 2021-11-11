#region Packages

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

#endregion

public class Manager : MonoBehaviour
{
    private static string saveFolder => Application.persistentDataPath + "/Saves/";

    [HideInInspector] public List<Label> labels = new List<Label>();
    [HideInInspector] public List<Ingredient> allIngredients = new List<Ingredient>();
    [HideInInspector] public List<Recipe> allRecipes = new List<Recipe>();

    #region Build In States

    private void Awake()
    {
        LoadFromSavePath();
    }

    private void OnApplicationQuit()
    {
        SaveToSavePath();
    }

    #endregion

    #region Internal

    [HorizontalGroup("Buttons", 0.5f)]
    [Button("Load")]
    private void LoadFromSavePath()
    {
        string loadString;

        #region Ingredients

        if (File.Exists(saveFolder + "Ingredients.txt"))
        {
            loadString = File.ReadAllText(saveFolder + "Ingredients.txt");
            if (loadString != "")
            {
                foreach (string s in loadString.Split(';'))
                {
                    object ingredient = JsonUtility.FromJson(s, typeof(Ingredient));
                    allIngredients.Add(ingredient as Ingredient);
                }
            }
        }
        else
            File.Create(saveFolder + "Ingredients.txt");

        #endregion

        #region Recipies

        if (!File.Exists(saveFolder + "Recipies.txt"))
        {
            loadString = File.ReadAllText(saveFolder + "Recipies.txt");
            if (loadString != "")
            {
                foreach (string s in loadString.Split(';'))
                {
                    object recipe = JsonUtility.FromJson(s, typeof(Recipe));
                    allRecipes.Add(recipe as Recipe);
                }
            }
        }
        else
            File.Create(saveFolder + "Recipies.txt");

        #endregion

        #region Labels

        
        if (!File.Exists(saveFolder + "Labels.txt"))
        {
            loadString = File.ReadAllText(saveFolder + "Labels.txt");
            if (loadString != "")
            {
                foreach (string s in loadString.Split(';'))
                {
                    object label = JsonUtility.FromJson(s, typeof(Label));
                    labels.Add(label as Label);
                }
            }
        }
        else
            File.Create(saveFolder + "Labels.txt");

        #endregion
    }

    [HorizontalGroup("Buttons/Right")]
    [Button("Save")]
    private void SaveToSavePath()
    {
        #region Ingredients

        string saveString = allIngredients.Aggregate("", (current, i) =>
            current + JsonUtility.ToJson(i) + ";");
        File.WriteAllText(saveFolder + "Ingredients.txt", saveString);

        #endregion

        #region Recipies

        saveString = allRecipes.Aggregate("", (current, i) =>
            current + JsonUtility.ToJson(i) + ";");
        File.WriteAllText(saveFolder + "Recipies.txt", saveString);

        #endregion

        #region Labels

        saveString = labels.Aggregate("", (current, i) =>
            current + JsonUtility.ToJson(i) + ";");
        File.WriteAllText(saveFolder + "Labels.txt", saveString);

        #endregion
    }

    #endregion
}