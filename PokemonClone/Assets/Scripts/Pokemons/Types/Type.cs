using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeName
{
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fighting,
    Poison,
    Ground,
    Flying,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy
}

[CreateAssetMenu(fileName = "Type", menuName = "Pokemon/Create new Type", order = 4)]
public class Type : ScriptableObject
{
    #region Values
    [SerializeField] private TypeName typeName = 0;
    [SerializeField] private Type[] noEffect = new Type[0];
    [SerializeField] private Type[] resistances = new Type[0];
    [SerializeField] private Type[] weakness = new Type[0];
    #endregion

    #region Getters
    public TypeName GetTypeName()
    {
        return typeName;
    }

    public bool GetNoEffect(TypeName input)
    {
        foreach (Type t in noEffect)
        {
            if (t.GetTypeName() == input)
                return true;
        }

        return false;
    }

    public int GetResistance(TypeName input)
    {
        int result = 0;

        foreach (Type t in resistances)
        {
            if (t.typeName == input)
                result += 1;
        }

        return result;
    }

    public int GetWeakness(TypeName input)
    {
        int result = 0;

        foreach (Type t in weakness)
        {
            if (t.typeName == input)
                result += 1;
        }

        return result;
    }
    #endregion
}