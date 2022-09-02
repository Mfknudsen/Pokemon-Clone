#region Packages

using System.Collections;
using System.Linq;
using Runtime.Battle.Actions;
using Runtime.Battle.Systems;
using Runtime.Battle.Systems.Interfaces;
using Runtime.Battle.Systems.Spots;
using Runtime.Battle.Systems.Static_Operations;
using Runtime.Pokémon;
using Runtime.Systems;
using Runtime.Weathers._Extra;
using Runtime.Weathers.Climate;
using UnityEngine;

#endregion

namespace Runtime.Weathers.Energy
{
    public class ThunderStormWeather : EnergyWeather, IOnTurnEnd, IAccuracyModify, IOperation
    {
        #region Values

        [SerializeField] private TypeName[] immuneTypes;
        [SerializeField] private Terrain elTerrain;
        [SerializeField] private GameObject lightingPrefab;
        private bool done;

        public override void Setup()
        {
            base.Setup();

            if (amplified)
                BattleManager.instance.GetWeatherManager().ApplyTerrain(elTerrain);
        }

        #endregion

        public bool Done()
        {
            return done;
        }

        public IEnumerator Operation()
        {
            done = false;
            OperationManager operationManager = OperationManager.instance;
            WeatherManager weatherManager = BattleManager.instance.GetWeatherManager();
            bool raining = weatherManager.GetAll()[0] is RainWeather;

            foreach (Spot spot in BattleManager.instance.GetSpotOversight().GetSpots())
            {
                if (Random.Range(0, 10) > 1)
                    continue;

                Pokemon pokemon = spot.GetActivePokemon();

                if (!raining && pokemon.GetTypes().Any(type => immuneTypes.Contains(type.GetTypeName())))
                    continue;
                
                //Damage
                OperationsContainer container = new();
                int damagePerTarget = pokemon.GetStat(Stat.HP) / 10;
                DamagePokemon damagePokemon = new(pokemon, damagePerTarget, 1);
                container.Add(damagePokemon);

                //Visual
                GameObject obj = Instantiate(lightingPrefab);
                ThunderStormLighting lighting = obj.GetComponent<ThunderStormLighting>();
                container.Add(lighting);

                operationManager.AddOperationsContainer(container);
            }

            done = true;

            yield break;
        }

        public void End()
        {
        }

        public float Effect(PokemonMove pokemonMove)
        {
            if (pokemonMove.GetMoveType().GetTypeName() != TypeName.Electric)
                return 1;

            pokemonMove.SetAccuracy(100);

            return 1;
        }
    }
}