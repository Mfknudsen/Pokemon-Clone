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
    [CreateAssetMenu(fileName = "Sandstorm", menuName = "Pokemon/Weather/Irritants/Sandstorm")]
    public class SandstormWeather : IrritantWeather, IOnTurnEnd, IStatModifier
    {
        [Header("On Turn End"), SerializeField]
        private TypeName ground, rock, steel;

        [Header("On Pokemon Enter"), SerializeField]
        private Type boostType;

        [SerializeField] private Stat boostStat;

        #region Interface Overrides

        private bool IsImmune(Pokemon pokemon)
        {
            return pokemon.GetTypes().Any(type => type.GetTypeName() == ground ||
                                                  type.GetTypeName() == steel ||
                                                  type.GetTypeName() == rock);
        }

        public IEnumerator Operation()
        {
            SpotOversight oversight = BattleManager.instance.GetSpotOversight();
            float secPerPokeMove = 200 * BattleManager.instance.GetSecPerPokeMove();

            foreach (Spot spot in oversight.GetSpots())
            {
                Pokemon pokemon = spot.GetActivePokemon();

                if (pokemon == null || IsImmune(pokemon))
                    continue;

                int partsDealt = amplified ? 8 : 16;

                int damagePerTarget = pokemon.GetStat(Stat.HP) / partsDealt;
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

        public void Trigger(Pokemon pokemon)
        {
            if (pokemon.GetTypes().Contains(boostType))
                pokemon.EffectMultiplierStage(1, boostStat);
        }

        public float Modify(Pokemon pokemon, Stat stat)
        {
            if (pokemon.GetTypes().Any(type => type.GetTypeName() == rock &&
                                               stat == Stat.SpDef))
                return 1.5f;

            return !amplified
                ? 1
                : pokemon.GetTypes().Any(type => type.GetTypeName() == steel ||
                                                 type.GetTypeName() == ground &&
                                                 stat == Stat.Defence)
                    ? 1.5f
                    : 1;
        }

        #endregion
    }
}