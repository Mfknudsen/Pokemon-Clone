#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

#region Enums
public enum NonVolatile { Poison, Sleep, Freeze, Paralysis, Burn, Fainted }
public enum Volatile { Confusion }
#endregion

public abstract class Condition : ScriptableObject
{
    #region Values
    [SerializeField] protected bool isInstantiated = false;
    [SerializeField] protected Pokemon affectedPokemon = null;
    [SerializeField] protected string conditionEffect = "";
    [SerializeField] protected bool active = false, done = false;

    #endregion

    #region Getters
    public virtual string GetConditionName()
    {
        Debug.Log(name + "\nGet Condition Name Need Override!");
        return "";
    }

    public virtual Condition GetCondition()
    {
        Debug.Log(name + "\nGet Condition Need Override!");
        return null;
    }

    public bool GetIsInstantiated()
    {
        return isInstantiated;
    }

    public string GetConditionEffect()
    {
        return conditionEffect;
    }

    public bool GetDone()
    {
        return done;
    }
    #endregion

    #region Setters
    public void SetIsInstantiated(bool set)
    {
        isInstantiated = set;
    }
    public void SetAffectedPokemon(Pokemon pokemon)
    {
        affectedPokemon = pokemon;
    }
    #endregion

    #region In
    public virtual void Reset()
    {

    }

    public virtual IEnumerator ActivateCondition(ConditionOversight activator)
    {
        Debug.Log("Activation of Condition needs override!");
        yield return null;
    }
    #endregion
}
