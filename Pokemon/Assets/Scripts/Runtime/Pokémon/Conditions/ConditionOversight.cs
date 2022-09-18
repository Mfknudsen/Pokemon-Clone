﻿#region Packages

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Battle.Systems;
using Runtime.Communication;
using Runtime.Pokémon.Conditions.Non_Volatiles;
using Runtime.Systems.Operation;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

// ReSharper disable ParameterHidesMember
namespace Runtime.Pokémon.Conditions
{
    [CreateAssetMenu(fileName = "ConditionOversight", menuName = "Condition/Create new Condition Oversight")]
    public class ConditionOversight : ScriptableObject
    {
        #region Values

        [SerializeField, Required] private OperationManager operationManager;
        [SerializeField, Required] private ChatManager chatManager;

        [SerializeField]
        private Condition nonVolatileStatus; //Burn, Freeze, Paralysis, Poison, Sleep. Only one can be active.

        [SerializeField] private List<VolatileCondition>
            volatileStatus = new(); //Bound, CantEscape, Confusion, Curse...

        [SerializeField] private bool done, isStunned;

        private Pokemon pokemon;

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

        public void Setup(Pokemon pokemon)
        {
            volatileStatus = new List<VolatileCondition>();
            this.pokemon = pokemon;
        }

        public void Reset()
        {
            done = false;
        }

        public void ApplyVolatileCondition(VolatileCondition condition)
        {
            if (volatileStatus.Any(volatileCondition =>
                volatileCondition.GetConditionName().Equals(condition.GetConditionName()))) return;

            volatileStatus.Add(condition);

            foreach (Ability ability in BattleManager.instance.GetAbilityOversight().GetAbilities())
            {
                if (ability.GetActive())
                    ability.TriggerDisable(AbilityTrigger.OnStatusChange, pokemon);
                else
                    ability.TriggerEnable(AbilityTrigger.OnStatusChange, pokemon);
            }
        }

        public void TryApplyNonVolatileCondition(NonVolatileCondition iNonVolatileCondition)
        {
            Condition condition = iNonVolatileCondition;

            if (condition is FaintedCondition)
            {
                Destroy(nonVolatileStatus);

                nonVolatileStatus = condition;

                nonVolatileStatus.SetAffectedPokemon(pokemon);
            }
            else
            {
                if (!(condition is null) && nonVolatileStatus is null)
                {
                    Destroy(nonVolatileStatus);
                    nonVolatileStatus = condition.GetCondition();
                    nonVolatileStatus.SetAffectedPokemon(pokemon);
                }
                else if (condition is null && !(nonVolatileStatus is null))
                {
                    if (!(nonVolatileStatus is FaintedCondition)) return;

                    Destroy(nonVolatileStatus);
                    nonVolatileStatus = null;
                }
            }

            foreach (Ability ability in BattleManager.instance.GetAbilityOversight().GetAbilities())
            {
                if (ability.GetActive())
                    ability.TriggerDisable(AbilityTrigger.OnStatusChange, pokemon);
                else
                    ability.TriggerEnable(AbilityTrigger.OnStatusChange, pokemon);
            }
        }

        public void RemoveFromCondition(Condition condition)
        {
            if (volatileStatus.Contains(condition))
            {
                volatileStatus.Remove(condition as VolatileCondition);
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

            #region Check To Play

            List<Condition> toPlay = new();
            if (nonVolatileStatus != null)
            {
                if (!nonVolatileStatus.GetBeforeAttack())
                    toPlay.Add(nonVolatileStatus);
            }

            toPlay.AddRange(volatileStatus
                .Where(volatileStatus => !volatileStatus.GetBeforeAttack()));

            #endregion

            #region Play All

            foreach (Condition condition in toPlay)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                if (!(condition is IOperation iOperation)) continue;

                OperationsContainer container = new();
                container.Add(iOperation);
                operationManager.AddOperationsContainer(container);

                while (!operationManager.GetDone() || !chatManager.GetIsClear())
                    yield return null;

                condition.Reset();
            }

            #endregion

            done = true;
        }

        public IEnumerator CheckConditionEndTurn()
        {
            done = false;

            List<Condition> toPlay = new();

            #region Check To Play

            if (!(nonVolatileStatus is null))
            {
                if (!nonVolatileStatus.GetBeforeAttack())
                    toPlay.Add(nonVolatileStatus.GetCondition());
            }

            toPlay.AddRange(volatileStatus
                .Where(volatileStatus => !toPlay.Contains(volatileStatus)));

            #endregion

            #region Play All

            foreach (Condition condition in toPlay)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                if (condition is not IOperation iOperation) continue;

                OperationsContainer container = new();
                container.Add(iOperation);
                operationManager.AddOperationsContainer(container);

                while (!operationManager.GetDone() || !chatManager.GetIsClear())
                    yield return null;

                condition.Reset();
            }

            #endregion

            done = true;
        }

        public void CheckFaintedCondition()
        {
            done = false;
            Condition condition = nonVolatileStatus;

            if (condition is FaintedCondition and IOperation iOperation)
            {
                OperationsContainer container = new();
                container.Add(iOperation);
                operationManager.AddOperationsContainer(container);
            }

            done = true;
        }

        #endregion
    }
}