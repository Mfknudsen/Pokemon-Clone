#region SDK

using UnityEngine;

#endregion

namespace Mfknudsen.Pokémon.Abilities
{
    [CreateAssetMenu(menuName = "Ability/Guts")]
    public class Guts : Ability
    {
        #region Values

        private Pokemon affectedPokemon;

        #endregion

        #region In

        public override void ReceiveInfo(object info)
        {
            if (info is Pokemon pokemon)
                affectedPokemon = pokemon;
        }

        // ReSharper disable once ParameterHidesMember
        public override void Trigger(AbilityTrigger abilityTrigger)
        {
            if(this.abilityTrigger != abilityTrigger) return;
            
            
        }

        #endregion
    }
}