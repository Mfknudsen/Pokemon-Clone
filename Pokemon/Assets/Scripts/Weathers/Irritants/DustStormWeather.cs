#region Packages

using System;
using System.Linq;
using Mfknudsen.Battle.Systems.Interfaces;
using Mfknudsen.Pokémon;
using UnityEngine;

#endregion

namespace Mfknudsen.Weathers.Irritants
{
    public class DustStormWeather : IrritantWeather, IStatModifier
    {
        #region Values

        [SerializeField] private TypeName boostType, reduceType;
        [SerializeField] private Stat boostStat, reduceStat;
        [SerializeField] private float boostValue, reduceValue;

        #endregion

        #region Interface Overrides

        public float Modify(Pokemon pokemon, Stat stat)
        {
            if (pokemon.GetTypes().Any(type => type.GetTypeName() == boostType) && stat == boostStat)
                return boostValue;

            if (pokemon.GetTypes().Any(type => type.GetTypeName() == reduceType) && stat == reduceStat)
                return reduceValue;

            return 1;
        }

        #endregion
    }
}