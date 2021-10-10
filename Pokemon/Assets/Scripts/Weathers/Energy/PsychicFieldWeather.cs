using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Battle.Systems.Interfaces;
using Mfknudsen.Pokémon;
using UnityEngine;

namespace Mfknudsen.Weathers.Energy
{
    public class PsychicFieldWeather : EnergyWeather, IFinalModifier
    {
        [SerializeField] private TypeName boostType, reduceType;

        [SerializeField] private Terrain psyTerrain;

        public override void Setup()
        {
            base.Setup();

            if (amplified)
                BattleManager.instance.GetWeatherManager().ApplyTerrain(psyTerrain);
        }

        public float Modify(PokemonMove pokemonMove)
        {
            if (pokemonMove.GetMoveType().GetTypeName() == boostType)
                return 1.5f;

            return pokemonMove.GetMoveType().GetTypeName() == reduceType
                ? 0.5f
                : 1;
        }
    }
}