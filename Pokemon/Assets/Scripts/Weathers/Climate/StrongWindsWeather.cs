using Mfknudsen.Battle.Systems;

namespace Mfknudsen.Weathers.Climate
{
    public class StrongWindsWeather : ClimateWeather
    {
        public override void Setup()
        {
            WeatherManager weatherManager = BattleManager.instance.GetWeatherManager();
            
            weatherManager.ApplyIrritant(null);
            weatherManager.ApplyEnergy(null);
            
            weatherManager.SetAmplified(true);
        }
    }
}