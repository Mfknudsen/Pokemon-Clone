#region Packages

using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Systems.Interfaces;
using Mfknudsen.Pokémon;
using UnityEngine;

#endregion

namespace Mfknudsen.Weathers.Climate
{
    public class BloodMoonWeather : ClimateWeather, IOnSuperEffective, IBypassStatus, IPriorityModify
    {
        #region Values

        [Header("Bypass Status"), SerializeField]
        private Type statusType;

        [Header("Priority Modify"), SerializeField]
        private Type priorityType;

        #endregion

        #region Interface Overrides

        //IOnSuperEffective
        float IOnSuperEffective.Effect(PokemonMove pokemonMove)
        {
            return 1.2f;
        }

        //IBypassStatus
        public bool CanEffect(Type type)
        {
            if (!amplified) return false;

            return type == statusType;
        }

        //IPriorityModify
        float IPriorityModify.Effect(PokemonMove pokemonMove)
        {
            bool isStatus = pokemonMove.GetCategory() == Category.Status;

            if (!isStatus)
                return 0;

            if (amplified)
                return 1;

            return priorityType == pokemonMove.GetMoveType()
                ? 1
                : 0;
        }

        #endregion
    }
}