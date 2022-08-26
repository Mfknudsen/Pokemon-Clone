#region Libraries

using Mfknudsen.Battle.Systems;
using Mfknudsen.Battle.Systems.Interfaces;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Pokémon;
using UnityEngine;
using Type = Mfknudsen.Pokémon.Type;

#endregion

namespace Mfknudsen.Weathers.Energy
{
    [CreateAssetMenu(fileName = "Battle Aura", menuName = "Pokemon/Weather/Energy/Battle Aura")]
    public class BattleAuraWeather : EnergyWeather, IBypassImmune, IOnPokemonEnter
    {
        #region Values

        [SerializeField] private Type type;

        #endregion

        public override void Setup()
        {
            base.Setup();

            foreach (Spot spot in BattleManager.instance.GetSpotOversight().GetSpots())
                IncreaseCritical(spot.GetActivePokemon());
        }

        #region Internal

        private static void IncreaseCritical(Pokemon pokemon)
        {
        }

        #endregion

        #region Interface Overrides

        //IBypassImmune
        public bool CanEffect(TypeName attackType, TypeName defendsType)
        {
            if (!amplified)
                return false;

            return (attackType == TypeName.Fighting || attackType == TypeName.Normal) &&
                   defendsType == TypeName.Ghost;
        }

        //IOnPokemonEnter
        public void Trigger(Pokemon pokemon)
        {
            IncreaseCritical(pokemon);
        }

        #endregion
    }
}