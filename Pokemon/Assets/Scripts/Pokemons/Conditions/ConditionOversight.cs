using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConditionOversight", menuName = "Condition/Create new Condition Oversight")]
public class ConditionOversight : ScriptableObject
{
    #region Values
    [Header("Object Reference:")]
    [SerializeField] private bool isInstantiated = false;
    [SerializeField] private Condition nonVolatileStatus = null;  //Burn, Freeze, Paralysis, Poison, Sleep. Only one can be active.
    [SerializeField] private List<Condition> volatileStatus = new List<Condition>();  //Bound, CantEscape, Confusion, Curse...

    [Header("Before Pokemon Move:")]
    [SerializeField] private NonVolatile[] beforeNonVolatile = new NonVolatile[0];
    [SerializeField] private Volatile[] beforeVolatile = new Volatile[0];

    [Header("End of turn:")]
    [SerializeField] private NonVolatile[] endNonVolatile = new NonVolatile[0];
    [SerializeField] private Volatile[] endVolatile = new Volatile[0];
    #endregion
    #region Getters
    public ConditionOversight GetConditionOversight()
    {
        ConditionOversight result = this;

        if (!result.GetIsInstantiated())
        {
            result = Instantiate(result);
            result.SetIsInstantiated(true);
        }

        return result;
    }

    public bool GetIsInstantiated()
    {
        return isInstantiated;
    }

    public Condition GetNonVolatileStatus()
    {
        return nonVolatileStatus;
    }
    #endregion
    #region Setters
    public void SetIsInstantiated(bool set)
    {
        isInstantiated = set;
    }
    #endregion
    #region In
    public bool TryApplyVolatileCondition(Condition condition)
    {
        foreach (Condition v in volatileStatus)
        {
            if (v.GetConditionName() == condition.GetConditionName())
                return false;
        }

        volatileStatus.Add(condition.GetCondition());
        return true;
    }
    public bool TryApplyNonVolatileCondition(Condition condition)
    {
        if (nonVolatileStatus == null)
        {
            nonVolatileStatus = condition.GetCondition();
            return true;
        }

        return false;
    }
    public void Reset()
    {

    }
    #endregion
    #region Out
    public bool CheckConditionBeforeMove()
    {
        #region For NonVolatile
        foreach (NonVolatile nonVol in beforeNonVolatile)
        {
            if (nonVolatileStatus.GetConditionName() == nonVol.ToString())
                nonVolatileStatus.ActivateCondition();
        }
        #endregion

        #region For Volatile
        foreach (Volatile vol in beforeVolatile)
        {
            foreach (Condition con in volatileStatus)
            {
                if (con.GetConditionName() == vol.ToString())
                    con.ActivateCondition();
            }
        }
        #endregion

        return true;
    }
    public bool CheckConditionEndTurn()
    {
        #region For NonVolatile
        if (nonVolatileStatus != null)
        {
            foreach (NonVolatile nonVol in endNonVolatile)
            {
                if (nonVolatileStatus.GetConditionName() == nonVol.ToString())
                    nonVolatileStatus.ActivateCondition();
            }
        }
        #endregion

        return true;
    }
    #endregion
}
