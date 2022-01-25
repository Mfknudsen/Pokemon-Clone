#region Packages

using System.Collections;
using System.Linq;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Battle.Systems.Interfaces;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Pokémon;
using UnityEngine;

#endregion

namespace Mfknudsen.Weathers.Climate
{
    [CreateAssetMenu(fileName = "Hail", menuName = "Pokemon/Weather/Irritants/Hail")]
    public class HailWeather : ClimateWeather, IOnTurnEnd
    {
        [SerializeField] private Type[] immuneTypes;

        #region Interface Overrides
        
        private bool IsImmune(Pokemon pokemon)
        {
            return pokemon.GetTypes().Any(type => immuneTypes.Contains(type));
        }

        #endregion

        public IEnumerator Operation()
        {
            SpotOversight oversight = BattleManager.instance.GetSpotOversight();
            float secPerPokeMove = 200 * BattleManager.instance.GetSecPerPokeMove();

            foreach (Spot spot in oversight.GetSpots())
            {
                Pokemon pokemon = spot.GetActivePokemon();

                if (pokemon == null || IsImmune(pokemon))
                    continue;

                int damagePerTarget = pokemon.GetStat(Stat.HP) / 16;
                float damageApplied = 0, damageOverTime = damagePerTarget / secPerPokeMove;


                while (damageApplied < damagePerTarget)
                {
                    if (damageApplied + damageOverTime >= damagePerTarget)
                        damageOverTime = damagePerTarget - damageApplied;

                    damageApplied += damageOverTime;

                    pokemon.ReceiveDamage(damageOverTime);

                    yield return new WaitForSeconds(BattleManager.instance.GetSecPerPokeMove() / secPerPokeMove);
                }
            }
        }
    }
}