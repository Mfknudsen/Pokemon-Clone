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

            if (this.amplified)
                BattleSystem.instance.GetWeatherManager().ApplyTerrain(this.psyTerrain);
        }

        public float Modify(PokemonMove pokemonMove)
        {
            if (pokemonMove.GetMoveType().GetTypeName() == this.boostType)
                return 1.5f;

            return pokemonMove.GetMoveType().GetTypeName() == this.reduceType
                ? 0.5f
                : 1;
        }
    }
}