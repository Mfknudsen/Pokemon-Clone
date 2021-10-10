#region Packages

using System.Linq;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Battle.Systems.Interfaces;
using Mfknudsen.Pokémon;
using Mfknudsen.Pokémon.Conditions;
using Mfknudsen.Pokémon.Conditions.Non_Volatiles;
using UnityEngine;

#endregion

// ReSharper disable SimplifyLinqExpressionUseAll
namespace Mfknudsen.Weathers.Irritants
{
    public class SmogWeather : IrritantWeather, IOnPokemonEnter
    {
        [SerializeField] private PoisonCondition poison;

        public override void Setup()
        {
            base.Setup();

            foreach (Pokemon pokemon in BattleManager.instance.GetSpotOversight().GetSpots()
                .Select(spot => spot.GetActivePokemon()).Where(pokemon =>
                    amplified || !pokemon.GetTypes().Any(type => type.GetTypeName() == TypeName.Poison)))
            {
                pokemon.GetConditionOversight().TryApplyNonVolatileCondition(poison);
            }
        }

        public void Trigger(Pokemon pokemon)
        {
            if (amplified || !pokemon.GetTypes().Any(type => type.GetTypeName() == TypeName.Poison))
                pokemon.GetConditionOversight().TryApplyNonVolatileCondition(poison);
        }
    }
}