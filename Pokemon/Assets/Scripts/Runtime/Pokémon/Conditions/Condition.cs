#region Packages

using Runtime.Communication;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.Pokémon.Conditions
{
    public abstract class Condition : ScriptableObject
    {
        #region Values

        [SerializeField, Required] protected ChatManager chatManager;
        
        private bool isInstantiated;
        protected ConditionOversight conditionOversight;

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

        #endregion

        #region In

        // ReSharper disable once ParameterHidesMember
        public void Setup(ConditionOversight conditionOversight)
        {
            this.conditionOversight = conditionOversight;
        }

        public abstract void Reset();

        #endregion
    }

    public abstract class NonVolatileCondition : Condition
    {
    }

    public abstract class VolatileCondition : Condition
    {
        public abstract bool CanIncrease();
        public abstract void Increase(Condition condition);
    }
}