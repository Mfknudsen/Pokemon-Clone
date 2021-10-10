#region SDK

using Mfknudsen.Battle.Systems.Interfaces;
using UnityEngine;

#endregion

namespace Mfknudsen.Pokémon.Abilities
{
    [CreateAssetMenu(menuName = "Ability/Levitate")]
    public class Levitate : Ability, IImmuneAttackType
    {
        [SerializeField] private TypeName immuneType;
        
        public bool MatchType(TypeName type)
        {
            return type == immuneType;
        }

        public override void TriggerEnable(AbilityTrigger trigger, Pokemon currentPokemon)
        {
        }

        public override void TriggerDisable(AbilityTrigger trigger, Pokemon currentPokemon)
        {
        }
    }
}
