#region SDK

using System;
using Mfknudsen.Battle.Actions;
using UnityEngine;

#endregion

namespace Mfknudsen.Pokémon
{
    #region Enums

    public enum AbilityTrigger
    {
        OnEnterBattle,
        OnExitBattle,
        OnFaint,
        OnDamageCalculation,
        OnAfterAttack,
        OnEndTurn
    }

    #endregion

    [CreateAssetMenu(fileName = "Ability", menuName = "Pokemon/Create New Ability", order = 2)]
    public abstract class Ability : ScriptableObject
    {
        #region Values

        [SerializeField] private int index;
        [SerializeField] private string abilityName;
        [SerializeField, TextArea] private string description;
        [SerializeField] protected AbilityTrigger abilityTrigger;

        #region Battle

        private bool active;

        #endregion

        #endregion

        #region Getters

        public bool GetActive()
        {
            return active;
        }

        #endregion

        #region Setters

        public void SetActive(bool set)
        {
            active = set;
        }

        #endregion

        #region In

        public abstract void ReceiveInfo(object info);

        public abstract void Trigger(AbilityTrigger abilityTrigger);

        #endregion
    }
}