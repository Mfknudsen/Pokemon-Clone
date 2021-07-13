#region SDK

using System.Collections;
using UnityEngine;

//Custom

#endregion


namespace Mfknudsen.Pokémon.Conditions
{
    public abstract class Condition : ScriptableObject
    {
        #region Values

        private bool isInstantiated;
        protected bool done;
        [SerializeField] protected string conditionName;
        [SerializeField] protected Pokemon affectedPokemon;
        [SerializeField] protected bool beforeAttack;

        #endregion

        #region Getters
        
        public string GetConditionName()
        {
            return conditionName;
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
        
        public bool GetDone()
        {
            return done;
        }

        public bool GetBeforeAttack()
        {
            return beforeAttack;
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

        public void SetBeforeAttack(bool set)
        {
            beforeAttack = set;
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

    public interface INonVolatile
    {
    }

    public interface IVolatile
    {
    }
}