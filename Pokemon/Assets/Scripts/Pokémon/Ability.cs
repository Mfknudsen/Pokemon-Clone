#region SDK

using UnityEngine;

#endregion

namespace Mfknudsen.Pokémon
{
    #region Enums

    public enum AbilityTrigger
    {
        OnEnterBattle,
        OnExitBattle,
        OnFainted,
        OnHitSelf,
        OnHitEnemy,
        OnHitAny,
        OnStatusEnable,
        OnStatusDisable,
        OnStatusChange
    }

    #endregion

    [CreateAssetMenu(fileName = "Ability", menuName = "Pokemon/Create New Ability", order = 2)]
    public abstract class Ability : ScriptableObject
    {
        #region Values

        [SerializeField] private int index;
        [SerializeField] private string abilityName;
        [SerializeField] protected AbilityTrigger enableTrigger, disableTrigger;
        [SerializeField, TextArea] private string description;

        protected Pokemon affectedPokemon;

        #region Battle

      [SerializeField]  private bool active;

        #endregion

        #endregion

        #region Getters

        public bool GetActive()
        {
            return active;
        }

        #endregion

        #region Setters

        public void SetAffectedPokemon(Pokemon pokemon)
        {
            affectedPokemon = pokemon;
        }

        public void SetActive(bool set)
        {
            active = set;
        }

        #endregion

        #region In

        public abstract void TriggerEnable(AbilityTrigger trigger, Pokemon currentPokemon);

        public abstract void TriggerDisable(AbilityTrigger trigger, Pokemon currentPokemon);

        #endregion
    }
}