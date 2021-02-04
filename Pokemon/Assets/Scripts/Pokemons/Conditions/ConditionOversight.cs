﻿#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#endregion

[CreateAssetMenu(fileName = "ConditionOversight", menuName = "Condition/Create new Condition Oversight")]
public class ConditionOversight : ScriptableObject
{
    #region Values
    [Header("Object Reference:")]
    [SerializeField] private bool isInstantiated = false;
    [SerializeField] private Condition nonVolatileStatus = null;  //Burn, Freeze, Paralysis, Poison, Sleep. Only one can be active.
    [SerializeField] private List<Condition> volatileStatus = new List<Condition>();  //Bound, CantEscape, Confusion, Curse...

    [Header("Condition Check:")]
    [SerializeField] private int conditionIndex = 0;
    [SerializeField] private bool done = false;

    [Header(" -- Before Pokemon Move:")]
    [SerializeField] private NonVolatile[] beforeNonVolatile = new NonVolatile[0];
    [SerializeField] private Volatile[] beforeVolatile = new Volatile[0];
    [SerializeField] private bool isStunned = false;

    [Header(" -- End of turn:")]
    [SerializeField] private NonVolatile[] endNonVolatile = new NonVolatile[0];
    [SerializeField] private Volatile[] endVolatile = new Volatile[0];
    #endregion

    #region Getters
    public ConditionOversight GetConditionOversight()
    {
        return this;
    }

    public bool GetIsInstantiated()
    {
        return isInstantiated;
    }

    public Condition GetNonVolatileStatus()
    {
        return nonVolatileStatus;
    }

    public bool GetDone()
    {
        return done;
    }

    public bool GetIsStunned()
    {
        return isStunned;
    }
    #endregion

    #region Setters
    public void SetIsInstantiated(bool set)
    {
        isInstantiated = set;

        #region Before
        beforeNonVolatile = new NonVolatile[3];
        beforeNonVolatile[0] = NonVolatile.Paralysis;
        beforeNonVolatile[1] = NonVolatile.Freeze;
        beforeNonVolatile[2] = NonVolatile.Sleep;

        beforeVolatile = new Volatile[1];
        beforeVolatile[0] = Volatile.Confusion;
        #endregion

        #region After
        endNonVolatile = new NonVolatile[2];
        endNonVolatile[0] = NonVolatile.Burn;
        endNonVolatile[1] = NonVolatile.Poison;
        #endregion
    }

    public void SetDone(bool set)
    {
        done = set;
    }

    public void SetIsStunned(bool set)
    {
        isStunned = set;
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
        if (nonVolatileStatus == null || condition.GetConditionName() == NonVolatile.Fainted.ToString())
        {
            nonVolatileStatus = condition.GetCondition();
            return true;
        }

        return false;
    }

    public void Reset()
    {
        conditionIndex = 0;
        done = false;
        isStunned = false;
    }

    public void RemoveFromCondition(Condition condition)
    {
        if (volatileStatus.Contains(condition))
        {
            volatileStatus.Remove(condition);
        }
        else if (nonVolatileStatus == condition)
        {
            nonVolatileStatus = null;
        }
    }
    #endregion

    #region Out
    public IEnumerator CheckConditionBeforeMove()
    {
        done = false;

        List<Condition> toPlay = new List<Condition>();

        #region Check To Play
        if (nonVolatileStatus != null)
        {
            foreach (NonVolatile v in beforeNonVolatile)
            {
                if (nonVolatileStatus.GetConditionName() == v.ToString())
                    toPlay.Add(nonVolatileStatus.GetCondition());
            }
        }
        foreach (Volatile v in beforeVolatile)
        {
            foreach (Condition c in volatileStatus)
            {
                if (c.GetConditionName() == v.ToString())
                    toPlay.Add(c.GetCondition());
            }
        }
        #endregion

        #region Play All
        if (toPlay.Count > 0)
        {
            BattleMaster.instance.SetConditionOperation(toPlay[conditionIndex].ActivateCondition(this));

            while (conditionIndex < toPlay.Count)
            {
                if (BattleMaster.instance.GetConditionOperation() == null && !done && toPlay[conditionIndex].GetDone())
                {
                    toPlay[conditionIndex].Reset();

                    conditionIndex++;
                    if (conditionIndex < toPlay.Count)
                        BattleMaster.instance.SetConditionOperation(toPlay[conditionIndex].ActivateCondition(this));
                }

                while (BattleMaster.instance.GetConditionOperation() != null)
                {
                    if (toPlay[conditionIndex].GetDone())
                        BattleMaster.instance.SetConditionOperation(null);
                    yield return null;
                }

                yield return null;
            }
        }
        #endregion

        done = true;
    }

    public IEnumerator CheckConditionEndTurn()
    {
        done = false;

        List<Condition> toPlay = new List<Condition>();

        #region Check To Play
        if (nonVolatileStatus != null)
        {
            foreach (NonVolatile v in endNonVolatile)
            {
                if (nonVolatileStatus.GetConditionName() == v.ToString())
                    toPlay.Add(nonVolatileStatus.GetCondition());
            }
        }
        foreach (Volatile v in endVolatile)
        {
            foreach (Condition c in volatileStatus)
            {
                if (c.GetConditionName() == v.ToString())
                    toPlay.Add(c.GetCondition());
            }
        }
        #endregion

        #region Play All
        if (toPlay.Count > 0)
        {
            Condition checker = null;

            while (conditionIndex < toPlay.Count)
            {
                if (BattleMaster.instance.GetConditionOperation() == null)
                {
                    toPlay[conditionIndex].Reset();

                    if (conditionIndex < toPlay.Count)
                    {
                        BattleMaster.instance.SetConditionOperation(toPlay[conditionIndex].ActivateCondition(this));
                        checker = toPlay[conditionIndex];

                        conditionIndex++;
                    }
                }

                while (checker != null)
                {
                    if (checker.GetDone())
                    {
                        BattleMaster.instance.SetConditionOperation(null);
                        checker = null;
                    }
                    yield return null;
                }

                yield return null;
            }
        }
        #endregion

        done = true;
    }

    public IEnumerator CheckFaintedCondition()
    {
        done = false;
        Condition c = nonVolatileStatus;

        if (c.GetConditionName() == NonVolatile.Fainted.ToString())
        {
            BattleMaster.instance.SetConditionOperation(c.ActivateCondition(this));

            while (c != null)
            {
                if (c.GetDone())
                {
                    BattleMaster.instance.SetConditionOperation(null);
                    c = null;
                }
                yield return null;
            }
        }

        yield return null;

        done = true;
    }
    #endregion
}