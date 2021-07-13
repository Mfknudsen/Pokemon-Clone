#region SDK

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Comunication;
using Mfknudsen.Pokémon;

#endregion

namespace Mfknudsen.Battle.Actions
{
    public abstract class BattleAction : ScriptableObject
    {
        #region Values

        [Header("Object Reference:")] [SerializeField]
        protected bool active;

        [SerializeField] protected bool done;

        [Header("Move Reference:")] [SerializeField]
        protected Pokemon currentPokemon;

        [SerializeField] protected List<Pokemon> targetPokemon;
        [SerializeField] protected bool moveActive;

        [Header("Priority:")] [SerializeField] protected int priority;
        [SerializeField] protected string[] priorityInteraction;

        [Header("Chat:")] [SerializeField] protected Chat[] chatOnActivation = new Chat[0];

        private bool isInstantiated = false;

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

        public bool GetDone()
        {
            return done;
        }

        public bool GetIsInstantiated()
        {
            return isInstantiated;
        }

        public int GetPriority()
        {
            return priority;
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
            Debug.Log("Transfering");
            return new Chat[0];
        }

        #endregion

        #region IEnumerator

        protected virtual IEnumerator Operation()
        {
            Debug.Log("Operation need Override!");
            yield return null;
        }

        #endregion
    }
}