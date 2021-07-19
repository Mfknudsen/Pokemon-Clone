#region SDK

using System.Collections;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Comunication;
using UnityEngine;

//Custom

#endregion

namespace Mfknudsen.Pokémon.Conditions
{
    [CreateAssetMenu(fileName = "Condition", menuName = "Condition/Create new Non-Volatile Condition/Poison",
        order = 1)]
    public class PoisonCondition : Condition, INonVolatile
    {
        #region Values

        [SerializeField] private bool badlyPoison;
        [SerializeField] private float damage;
        [SerializeField] private float n, increaseN = 1;
        [SerializeField] private Chat onEffectChat;

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

        public override IEnumerator ActivateCondition(ConditionOversight activator)
        {
            Chat toSend = onEffectChat.GetChat();
            toSend.AddToOverride("<TARGET_NAME>", affectedPokemon.GetName());
            ChatMaster.instance.Add(toSend);

            if (damage == 0)
                SetDamage(affectedPokemon.GetStat(Stat.HP));

            damage = GetDamage();
            float divide = 200;
            float reletivSpeed = BattleMaster.instance.GetSecPerPokeMove() / divide;
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

        #endregion
    }
}