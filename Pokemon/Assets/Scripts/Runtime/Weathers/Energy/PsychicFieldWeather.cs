using Runtime.Battle.Actions;
using Runtime.Battle.Systems;
using Runtime.Battle.Systems.Interfaces;
using Runtime.Pokémon;
using UnityEngine;

namespace Runtime.Weathers.Energy
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