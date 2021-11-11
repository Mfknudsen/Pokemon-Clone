#region MyRegion

using UnityEngine.UI;

#endregion

[System.Serializable]
public class Ingredient
{
    public int id { get; }
    public Image visual;
    public string ingredientName;
    public Nutrition nutrition { get; }

    public Ingredient()
    {
        nutrition = new Nutrition();
    }
}
