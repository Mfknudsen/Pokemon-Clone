#region Packages

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Weathers;

#endregion

namespace Mfknudsen.Battle.Systems
{
    public class WeatherManager
    {
        #region Values

        private ClimateWeather climateWeather;
        private IrritantWeather irritantWeather;
        private EnergyWeather energyWeather;
        private Terrain terrain;

        private bool amplifyNext = false;

        #endregion

        #region Getters

        public Weather[] GetAll()
        {
            return new Weather[] { climateWeather, irritantWeather, energyWeather };
        }

        public List<T> GetWeatherWithInterface<T>()
        {
            List<T> result = new List<T>();

            foreach (Weather weather in GetAll())
            {
                if (weather is T t)
                    result.Add(t);
            }

            return result;
        }

        public bool GetAmplified()
        {
            if (!amplifyNext) return false;

            amplifyNext = false;
            return true;
        }

        #endregion

        #region Setters

        public void SetAmplified(bool set)
        {
            amplifyNext = set;
        }

        #endregion

        #region In

        public void ApplyClimate(ClimateWeather set)
        {
            ShiftWeather(climateWeather, set);
            climateWeather = set;
        }

        public void ApplyIrritant(IrritantWeather set)
        {
            ShiftWeather(irritantWeather, set);
            irritantWeather = set;
        }

        public void ApplyEnergy(EnergyWeather set)
        {
            ShiftWeather(energyWeather, set);
            energyWeather = set;
        }

        public void ApplyTerrain(Terrain set)
        {
            terrain = set;
            terrain.Start();
        }

        #endregion

        #region Internal

        private void ShiftWeather(Weather current, Weather next)
        {
            BattleManager.instance.StartCoroutine(Shift());
        }

        #endregion

        #region IEnumerator

        private IEnumerator Shift()
        {
            yield return 0;
        }

        #endregion
    }
}