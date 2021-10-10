#region Packages

using System.Linq;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Systems.Interfaces;
using Mfknudsen.Pokémon;
using UnityEngine;

#endregion

namespace Mfknudsen.Weathers.Climate
{
    public class FogWeather : ClimateWeather, IAccuracyModify, IFinalModifier
    {
        #region Values

        [Header("Move Modification"), SerializeField]
        private Type[] typesUnaffected;

        [Header("Move Modification"), SerializeField]
        private SpecialAddons addonEffected;

        #endregion

        #region Interface Overrides

        //IAccuracyModify
        public bool CanEffect(PokemonMove pokemonMove)
        {
            return pokemonMove.GetAccuracy() != -1 || typesUnaffected.Contains(pokemonMove.GetMoveType());
        }

        public float Effect(PokemonMove pokemonMove)
        {
            return 0.8f;
        }

        //IFinalModifier
        public float Modify(PokemonMove pokemonMove)
        {
            return pokemonMove.GetSpecialAddons().Contains(addonEffected) ? 1.5f : 1;
        }

        #endregion
    }
}