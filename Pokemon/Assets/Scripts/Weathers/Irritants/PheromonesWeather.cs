#region Packages

using System.Linq;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Battle.Systems.Interfaces;
using Mfknudsen.Pokémon;
using Mfknudsen.Pokémon.Conditions;
using UnityEngine;

#endregion

namespace Mfknudsen.Weathers.Irritants
{
    public class PheromonesWeather : IrritantWeather, IStatModifier, IOnPokemonEnter
    {
        #region Values

        [Header("Stat Modifier"), SerializeField]
        private TypeName[] affectedTypes;

        [SerializeField] private Stat[] affectedStats;

        [Header("On Pokemon Enter"), SerializeField]
        private VolatileCondition confusion;

        #endregion

        public override void Setup()
        {
            base.Setup();

            foreach (Pokemon pokemon in BattleManager.instance.GetSpotOversight().GetSpots()
                .Select(spot => spot.GetActivePokemon()).Where(pokemon =>
                    pokemon.GetTypes().Any(type => affectedTypes.Contains(type.GetTypeName()))))
            {
                pokemon.GetConditionOversight().ApplyVolatileCondition(confusion);
            }
        }

        #region Interface Overrides

        //IStatModifier
        public float Modify(Pokemon pokemon, Stat stat)
        {
            return affectedStats.Contains(stat) &&
                   pokemon.GetTypes().Any(type => affectedTypes.Contains(type.GetTypeName()))
                ? 1.25f
                : 1;
        }

        //IOnPokemonEnter
        public void Trigger(Pokemon pokemon)
        {
        }

        #endregion
    }
}