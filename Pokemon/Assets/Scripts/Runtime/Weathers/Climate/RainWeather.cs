#region Libraries

using System.Linq;
using Runtime.Battle.Systems.Interfaces;
using Runtime.Pokémon;
using UnityEngine;
using Type = Runtime.Pokémon.Type;

#endregion

namespace Runtime.Weathers.Climate
{
    [CreateAssetMenu(fileName = "Rain", menuName = "Pokemon/Weather/Climate/Rain")]
    public class RainWeather : ClimateWeather, IStatModifier, IOnPokemonEnter
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

        #region Interface Overrides

        public float Modify(Pokemon pokemon, Stat stat)
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (!pokemon.GetTypes().Contains(types[i]) || stats[i] != stat) continue;

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