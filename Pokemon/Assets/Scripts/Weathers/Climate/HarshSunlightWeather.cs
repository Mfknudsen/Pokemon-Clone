#region Packages

using System.Linq;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Battle.Systems.Interfaces;
using Mfknudsen.Pokémon;
using UnityEngine;

#endregion

namespace Mfknudsen.Weathers.Climate
{
    public class HarshSunlightWeather : ClimateWeather, IStatModifier, IOnPokemonEnter
    {
        #region Values

        [Header("Stat Modifier"), SerializeField]
        private Type[] types;

        [SerializeField] private Stat[] stats;
        [SerializeField] private float[] changes;
        private float change;

        [Header("On Pokemon Enter"), SerializeField]
        private Type type;

        [SerializeField] private Stat stat;

        #endregion

        public override void Setup()
        {
            amplified = BattleManager.instance.GetWeatherManager().GetAmplified();
        }

        #region Interface Overrides

        // ReSharper disable once ParameterHidesMember
        public float Modify(Pokemon pokemon, Stat stat)
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (!pokemon.GetTypes().Contains(types[i]) || stat != stats[i]) continue;

                change = changes[i];
                return amplified ? change * 1.5f : change;
            }

            return 1;
        }

        public void Trigger(Pokemon pokemon)
        {
            if (pokemon.GetTypes().Contains(type))
                pokemon.EffectMultiplierStage(1, stat);
        }

        #endregion
    }
}