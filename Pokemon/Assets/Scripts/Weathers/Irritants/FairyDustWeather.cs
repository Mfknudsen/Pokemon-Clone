#region Packages

using System.Collections;
using System.Linq;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Battle.Systems.Interfaces;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Pokémon;
using UnityEngine;

#endregion

namespace Mfknudsen.Weathers.Irritants
{
    public class FairyDustWeather : IrritantWeather, IStatModifier, IOnTurnEnd
    {
        #region Values

        [SerializeField] private Terrain misty;

        [Header("Stat Modifier"), SerializeField]
        private TypeName[] noEffectTypes;

        #endregion

        public override void Setup()
        {
            base.Setup();

            if (amplified)
                BattleManager.instance.GetWeatherManager().ApplyTerrain(misty);
        }

        #region Interface Overrides

        //IStatModifier
        public float Modify(Pokemon pokemon, Stat stat)
        {
            return pokemon.GetTypes().Any(type => type.GetTypeName() != TypeName.Fairy) && stat == Stat.Evasion
                ? 0.8f
                : 1;
        }

        //IOnTurnEnd
        public IEnumerator Operation()
        {
            float secPerPokeMove = 200 * BattleManager.instance.GetSecPerPokeMove();

            foreach (Spot spot in BattleManager.instance.GetSpotOversight().GetSpots())
            {
                Pokemon pokemon = spot.GetActivePokemon();

                if (pokemon.GetTypes().Any(type => noEffectTypes.Contains(type.GetTypeName())))
                    continue;

                int damagePerTarget = pokemon.GetStat(Stat.HP) / 16;
                float damageApplied = 0, damageOverTime = damagePerTarget / secPerPokeMove;


                while (damageApplied < damagePerTarget)
                {
                    if (damageApplied + damageOverTime >= damagePerTarget)
                        damageOverTime = damagePerTarget - damageApplied;

                    damageApplied += damageOverTime;

                    pokemon.ReceiveDamage(-damageOverTime);

                    yield return new WaitForSeconds(BattleManager.instance.GetSecPerPokeMove() / secPerPokeMove);
                }
            }
        }

        #endregion
    }
}