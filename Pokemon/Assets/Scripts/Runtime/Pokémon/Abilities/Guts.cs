#region SDK

using Runtime.Battle.Systems.Interfaces;
using Runtime.Pokémon.Conditions;
using Runtime.Pokémon.Conditions.Non_Volatiles;
using UnityEngine;

#endregion

namespace Runtime.Pokémon.Abilities
{
    [CreateAssetMenu(menuName = "Ability/Guts")]
    public class Guts : Ability, IBurnStop, IStatModifier
    {
        #region Values

        [SerializeField] private float damageModification;

        #endregion

        #region In

        public override void TriggerEnable(AbilityTrigger trigger, Pokemon currentPokemon)
        {
            if (trigger != enableTrigger) return;

            Condition condition = affectedPokemon.GetConditionOversight().GetNonVolatileStatus();

            if (!(condition is null) && !(condition is FaintedCondition))
            {
                Debug.Log("Trigger");
                SetActive(true);
            }
        }

        public override void TriggerDisable(AbilityTrigger trigger, Pokemon currentPokemon)
        {
            if (trigger != enableTrigger) return;

            Condition condition = affectedPokemon.GetConditionOversight().GetNonVolatileStatus();

            if (condition is null || condition is FaintedCondition)
                SetActive(false);
        }

        #endregion

        #region Out

        public bool CanStopBurn(Pokemon pokemon)
        {
            return pokemon == affectedPokemon;
        }

        public float Modify(Pokemon pokemon, Stat stat)
        {
            return (pokemon == affectedPokemon && stat == Stat.Attack && GetActive()) ? damageModification : 1;
        }

        #endregion
    }
}