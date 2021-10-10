using System.Collections;
using DG.Tweening;
using Mfknudsen.Pokémon;
using UnityEngine;

namespace Mfknudsen.Battle.Systems.Static_Operations
{
    public class DamagePokemon : IOperation
    {
        private bool done = false;
        private readonly float damage, totalTime, splitTime = 200;
        private readonly Pokemon target;

        public DamagePokemon(Pokemon target, float damage, float totalTime)
        {
            this.damage = damage;
            this.totalTime = totalTime;
            this.target = target;
        }

        public bool Done()
        {
            return done;
        }

        public IEnumerator Operation()
        {
            done = false;
            
            float damageApplied = 0, damageOverTime = damage / splitTime;

            while (damageApplied < damage)
            {
                if (damageApplied + damageOverTime >= damage)
                    damageOverTime = damage - damageApplied;
                
                damageApplied += damageOverTime;

                target.ReceiveDamage(damageOverTime);

                yield return new WaitForSeconds(totalTime / splitTime);
            }

            done = true;
        }

        public void End()
        {
        }
    }

    public struct DamageProfile
    {
        
    }
}
