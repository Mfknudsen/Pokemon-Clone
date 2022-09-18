#region Packages

using System.Collections;
using System.Linq;
using Runtime.Battle.Systems;
using Runtime.Battle.Systems.Interfaces;
using Runtime.Battle.Systems.Spots;
using Runtime.Pokémon;
using Runtime.Pokémon.Conditions;
using Runtime.Systems.Operation;
using UnityEngine;

#endregion

namespace Runtime.Weathers.Energy
{
    public class CursedWindsWeather : EnergyWeather, IOnTurnEnd, IBeforeAction, IOperation
    {
        [SerializeField] private VolatileCondition flinch;

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private bool IsImmune(Pokemon pokemon)
        {
            return pokemon.GetTypes().Any(type => type.GetTypeName() == TypeName.Ghost ||
                                                  type.GetTypeName() == TypeName.Dark ||
                                                  type.GetTypeName() == TypeName.Normal);
        }

        public bool IsOperationDone()
        {
            throw new System.NotImplementedException();
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
                
                int damagePerTarget = pokemon.GetCalculatedStat(Stat.HP) / 16;
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

        public void OperationEnd()
        {
        }

        public void Modify(Pokemon pokemon)
        {
            if (IsImmune(pokemon)) return;

            if (Random.Range(0, 100) <= 10)
                pokemon.GetConditionOversight().ApplyVolatileCondition(flinch);
        }
    }
}