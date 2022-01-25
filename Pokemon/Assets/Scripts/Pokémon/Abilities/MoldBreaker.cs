using Mfknudsen.Battle.Systems.Interfaces;
using UnityEngine;

namespace Mfknudsen.Pokémon.Abilities
{
    [CreateAssetMenu(menuName = "Ability/Mold Breaker", fileName = "Mold Breaker")]
    public class MoldBreaker : Ability, IIgnoreAbilities
    {
        public override void TriggerEnable(AbilityTrigger trigger, Pokemon currentPokemon)
        {
        }

        public override void TriggerDisable(AbilityTrigger trigger, Pokemon currentPokemon)
        {
        }
    }
}
