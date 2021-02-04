﻿#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "Condition", menuName = "Condition/Create new Non-Volatile Condition/Burn", order = 1)]
public class BurnCondition : Condition
{
    #region Values
    [SerializeField] private NonVolatile conditionName = 0;
    [SerializeField] private float damage = 0;
    [SerializeField] private float n = 0, increaseN = 1;
    #endregion

    #region Getters
    public override string GetConditionName()
    {
        return conditionName.ToString();
    }

    public override Condition GetCondition()
    {
        Condition result = this;

        if (!result.GetIsInstantiated())
        {
            result = Instantiate(this);
            result.SetIsInstantiated(true);
        }

        return result;
    }

    public float GetDamage()
    {
        n += increaseN;
        return damage * n;
    }
    #endregion

    #region Setters
    public void SetDamage(int maxHP)
    {
        damage = maxHP / 16;
    }
    #endregion

    #region In
    public override IEnumerator ActivateCondition(ConditionOversight activator)
    {
        yield return null;
    }
    #endregion
}