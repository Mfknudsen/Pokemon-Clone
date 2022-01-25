#region Packages

using System.Linq;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Battle.Systems.Interfaces;
using Mfknudsen.Pokémon;
using UnityEngine;

#endregion

namespace Mfknudsen.Weathers.Irritants
{
    public class PollenStormWeather : IrritantWeather, IStatModifier
    {
        #region Values

        [SerializeField] private TypeName[] immuneTypes;
        [SerializeField] private Terrain terrain;

        #endregion

        public override void Setup()
        {
            base.Setup();


            if (amplified)
                BattleManager.instance.GetWeatherManager().ApplyTerrain(terrain);
        }

        #region Interface Overrides

        public float Modify(Pokemon pokemon, Stat stat)
        {
            return pokemon.GetTypes().Any(type => immuneTypes.Contains(type.GetTypeName()) && stat == Stat.SpAtk)
                ? 0.75f
                : 1;
        }

        #endregion
    }
}