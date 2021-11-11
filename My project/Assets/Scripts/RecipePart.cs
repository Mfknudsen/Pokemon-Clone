#region Packages

using System.Collections.Generic;

#endregion

[System.Serializable]
public class RecipePart
{
    public string partName = "New Part";
    public string partDescription = "New Description";
    public int percentOfRecipe = 100;

    private List<Ingredient> ingredients = new List<Ingredient>();
}