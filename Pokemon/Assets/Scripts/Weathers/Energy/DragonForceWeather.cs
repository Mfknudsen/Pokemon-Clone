#region Packages

using System.Collections;
using System.Linq;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Battle.Systems.Interfaces;
using Mfknudsen.Pokémon;
using UnityEngine;

#endregion

namespace Mfknudsen.Weathers.Energy
{
    public class DragonForceWeather : EnergyWeather, IOnSuperEffective, IOnMoveHit, IOperation
    {
        #region Values

        private bool hasHit;
        private Pokemon affected;

        #endregion

        public float Effect(PokemonMove pokemonMove)
        {
            if (!amplified)
                return 0.8f;

            return pokemonMove.GetMoveType().GetTypeName() == TypeName.Dragon
                ? 1.5f
                : 1.3f;
        }

        public bool Done()
        {
            throw new System.NotImplementedException();
        }

        public bool MultiHit(PokemonMove pokemonMove)
        {
            if (pokemonMove.GetCurrentPokemon() == affected)
                hasHit = true;
            else
            {
                affected = pokemonMove.GetCurrentPokemon();
                hasHit = false;
            }

            return hasHit;
        }

        public IEnumerator Operation()
        {
            float secPerPokeMove = 200 * BattleManager.instance.GetSecPerPokeMove();
            
            if (affected == null || affected.GetTypes()
                .Any(type => type.GetTypeName() == TypeName.Dragon)) yield break;

            int damagePerTarget = affected.GetStat(Stat.HP) / 10;
            float damageApplied = 0, damageOverTime = damagePerTarget / secPerPokeMove;

            while (damageApplied < damagePerTarget)
            {
                if (damageApplied + damageOverTime >= damagePerTarget)
                    damageOverTime = damagePerTarget - damageApplied;

                damageApplied += damageOverTime;

                affected.ReceiveDamage(damageOverTime);

                yield return new WaitForSeconds(BattleManager.instance.GetSecPerPokeMove() / secPerPokeMove);
            }
        }

        public void End()
        {
        }
    }
}