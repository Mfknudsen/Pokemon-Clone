#region Packages

using System;
using System.Collections.Generic;

#endregion

[Serializable]
public class Recipe
{
    public int id { get; }
    public string recipeName = "New Recipe";
    public List<string> labels = new List<string>();
    private readonly List<RecipePart> recipeParts = new List<RecipePart>();

    #region Getters

    public RecipePart[] GetRecipeParts()
    {
        return recipeParts.ToArray();
    }

    #endregion
    
    #region In

    public void AddRecipePart()
    {
        recipeParts.Add(new RecipePart());
    }

    public void RemoveRecipePart(int index)
    {
        recipeParts.RemoveAt(index);
    }

    #endregion
}