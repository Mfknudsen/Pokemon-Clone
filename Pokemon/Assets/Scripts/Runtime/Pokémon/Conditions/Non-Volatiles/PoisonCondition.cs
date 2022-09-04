#region Packages

using System.Collections;
using Runtime.Battle.Systems;
using Runtime.Communication;
using Runtime.Systems;
using UnityEngine;

#endregion

namespace Runtime.Pokémon.Conditions.Non_Volatiles
{
    [CreateAssetMenu(fileName = "Condition", menuName = "Condition/Create new Non-Volatile Condition/Poison",
        order = 1)]
    public class PoisonCondition : NonVolatileCondition, IOperation
    {
        #region Values

        [SerializeField] private bool badlyPoison;
        [SerializeField] private float damage;
        [SerializeField] private float n, increaseN = 1;
        [SerializeField] private Chat onEffectChat;

        private bool done;
        
        #endregion

        #region Getters

        public override Condition GetCondition()
        {
            Condition result = this;

            if (!result.GetIsInstantiated())
            {
                result = Instantiate(this);
                result.SetIsInstantiated(true);
            }

            return result;
        }

        public float GetDamage()
        {
            float result = damage * n;

            if (badlyPoison)
                n += increaseN;

            return result;
        }

        #endregion

        #region Setters

        public void SetDamage(int maxHP)
        {
            damage = maxHP / 16;
        }

        public void SetBadlyPoison(bool set)
        {
            badlyPoison = set;
        }

        #endregion

        #region In

        public override void Reset()
        {
            done = false;
        }

        public bool Done()
        {
            return done;
        }

        public IEnumerator Operation()
        {
            Chat toSend = onEffectChat.GetChat();
            toSend.AddToOverride("<TARGET_NAME>", affectedPokemon.GetName());
            ChatManager.instance.Add(toSend);

            if (damage == 0)
                SetDamage(affectedPokemon.GetCalculatedStat(Stat.HP));

            damage = GetDamage();
            float divide = 200;
            float reletivSpeed = BattleManager.instance.GetSecPerPokeMove() / divide;
            float relativeDamage = damage / divide;
            float appliedDamage = 0;

            while (appliedDamage < damage)
            {
                appliedDamage += relativeDamage;

                affectedPokemon.ReceiveDamage(relativeDamage);

                yield return new WaitForSeconds(reletivSpeed);
            }

            done = true;
        }

        public void End()
        {
        }

        #endregion
    }
}