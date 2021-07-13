#region SDK

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Comunication;
using UnityEngine;

#endregion

namespace Mfknudsen.Pokémon.Conditions
{
    [CreateAssetMenu(fileName = "ConditionOversight", menuName = "Condition/Create new Condition Oversight")]
    public class ConditionOversight : ScriptableObject
    {
        #region Values

        [SerializeField]
        private Condition nonVolatileStatus; //Burn, Freeze, Paralysis, Poison, Sleep. Only one can be active.

        [SerializeField]
        private List<Condition> volatileStatus = new List<Condition>(); //Bound, CantEscape, Confusion, Curse...

        [SerializeField] private bool done, isStunned;

        #endregion

        #region Getters

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

        public void SetIsStunned(bool set)
        {
            isStunned = set;
        }

        #endregion

        #region In

        public bool TryApplyVolatileCondition(IVolatile iVolatile)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            Condition condition = (Condition) iVolatile;
            foreach (Condition v in volatileStatus)
            {
                if (v.GetConditionName() == condition.GetConditionName())
                    return false;
            }

            volatileStatus.Add(condition.GetCondition());
            return true;
        }

        public void TryApplyNonVolatileCondition(INonVolatile iNonVolatile)
        {
            Condition condition = (Condition) iNonVolatile;
            if (!(condition is null) && nonVolatileStatus is null)
            {
                Destroy(nonVolatileStatus);
                nonVolatileStatus = condition.GetCondition();
            }
            else if (condition is null && !(nonVolatileStatus is null))
            {
                if (!(nonVolatileStatus is FaintedCondition)) return;

                Destroy(nonVolatileStatus);
                nonVolatileStatus = null;
            }
        }

        public void Reset()
        {
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

        public void ResetConditionList()
        {
            volatileStatus.Clear();
        }

        #endregion

        #region Out

        public IEnumerator CheckConditionBeforeMove()
        {
            done = false;

            List<Condition> toPlay = new List<Condition>();

            /*
            #region Check To Play

            if (nonVolatileStatus != null)
            {
                foreach (INonVolatile v in beforeNonVolatile)
                {
                    if (nonVolatileStatus.GetConditionName() == v.ToString())
                        toPlay.Add(nonVolatileStatus.GetCondition());
                }
            }

            foreach (IVolatile v in beforeVolatile)
            {
                foreach (Condition c in volatileStatus)
                {
                    if (c.GetConditionName() == v.ToString())
                        toPlay.Add(c.GetCondition());
                }
            }

            #endregion
*/

            #region Play All

            foreach (Condition condition in toPlay)
            {
                BattleMaster.instance.StartCoroutine(condition.ActivateCondition(this));

                while (!condition.GetDone() || !ChatMaster.instance.GetIsClear())
                    yield return null;

                condition.Reset();
            }

            #endregion

            done = true;
        }

        public IEnumerator CheckConditionEndTurn()
        {
            done = false;

            List<Condition> toPlay = new List<Condition>();

            /*
            #region Check To Play

            if (nonVolatileStatus != null)
            {
                foreach (INonVolatile v in endNonVolatile)
                {
                    if (nonVolatileStatus.GetConditionName() == v.ToString())
                        toPlay.Add(nonVolatileStatus.GetCondition());
                }
            }

            foreach (IVolatile v in endVolatile)
            {
                foreach (Condition c in volatileStatus)
                {
                    if (c.GetConditionName() == v.ToString())
                        toPlay.Add(c.GetCondition());
                }
            }

            #endregion
*/

            #region Play All

            foreach (Condition condition in toPlay)
            {
                BattleMaster.instance.StartCoroutine(condition.ActivateCondition(this));

                while (!condition.GetDone() || !ChatMaster.instance.GetIsClear())
                    yield return null;

                condition.Reset();
            }

            #endregion

            done = true;
        }

        public IEnumerator CheckFaintedCondition()
        {
            done = false;
            Condition condition = nonVolatileStatus;

            if (condition is FaintedCondition)
            {
                BattleMaster.instance.StartCoroutine(condition.ActivateCondition(this));

                while (!condition.GetDone() || !ChatMaster.instance.GetIsClear())
                    yield return null;
            }

            done = true;
        }

        #endregion
    }
}