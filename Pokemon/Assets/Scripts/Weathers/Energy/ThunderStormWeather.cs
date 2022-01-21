#region Packages

using System.Collections;
using System.Linq;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Battle.Systems.Interfaces;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Battle.Systems.Static_Operations;
using Mfknudsen.Pokémon;
using Mfknudsen.Weathers._Extra;
using Mfknudsen.Weathers.Climate;
using UnityEngine;

#endregion

namespace Mfknudsen.Weathers.Energy
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
            OperationManager operationManager = OperationManager.Instance;
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
                OperationsContainer container = new OperationsContainer();
                int damagePerTarget = pokemon.GetStat(Stat.HP) / 10;
                DamagePokemon damagePokemon = new DamagePokemon(pokemon, damagePerTarget, 1);
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