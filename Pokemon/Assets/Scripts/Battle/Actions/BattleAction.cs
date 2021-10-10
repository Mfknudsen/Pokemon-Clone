#region SDK

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Communication;
using Mfknudsen.Pokémon;

#endregion

namespace Mfknudsen.Battle.Actions
{
    public abstract class BattleAction : ScriptableObject, IOperation
    {
        #region Values

        [SerializeField] protected bool active, done;
        [SerializeField] private bool defaultTargetEnemy = true;

        [SerializeField] protected Pokemon currentPokemon;

        [SerializeField] protected List<Pokemon> targetPokemon;
        [SerializeField] protected bool moveActive;

        [SerializeField] protected int priority;
        [SerializeField] protected string[] priorityInteraction;
        [SerializeField] protected Chat[] chatOnActivation = Array.Empty<Chat>();

        private bool isInstantiated;

        #endregion

        #region Getters

        public virtual BattleAction GetAction()
        {
            return this;
        }

        public Pokemon GetCurrentPokemon()
        {
            return currentPokemon;
        }

        public bool GetIsInstantiated()
        {
            return isInstantiated;
        }

        public int GetPriority()
        {
            return priority;
        }

        public bool GetDefaultTargetEnemy()
        {
            return defaultTargetEnemy;
        }

        #endregion

        #region Setter

        public void SetTargets(Pokemon pokemon)
        {
            targetPokemon ??= new List<Pokemon>();

            targetPokemon.Add(pokemon);
        }

        public void SetCurrentPokemon(Pokemon pokemon)
        {
            currentPokemon = pokemon;
        }

        public void SetIsInstantiated(bool set)
        {
            isInstantiated = set;
        }

        public void SetPriority(int set)
        {
            priority = set;
        }

        #endregion

        #region Out

        public virtual IEnumerator Activate()
        {
            Debug.Log("Activate need Override!");
            return Operation();
        }

        #endregion

        #region Internal

        protected void SetupChats()
        {
            for (int i = 0; i < chatOnActivation.Length; i++)
                chatOnActivation[i] = Instantiate(chatOnActivation[i]);
        }

        protected virtual Chat[] TransferInformationToChat()
        {
            Debug.Log("Transferring");
            return Array.Empty<Chat>();
        }

        #endregion

        #region IOperation

        public bool Done()
        {
            return done;
        }

        public virtual IEnumerator Operation()
        {
            Debug.Log("Operation need Override!");
            yield return null;
        }

        public void End()
        {
        }

        #endregion
    }
}