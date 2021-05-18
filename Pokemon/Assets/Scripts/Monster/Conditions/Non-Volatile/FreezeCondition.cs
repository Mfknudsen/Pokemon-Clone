#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Custom
using Communications;
#endregion

namespace Monster.Conditions.Non_Volatile
{
    [CreateAssetMenu(fileName = "Condition", menuName = "Condition/Create new Non-Volatile Condition/Freeze", order = 1)]
    public class FreezeCondition : Condition
    {
        #region Values
        [SerializeField] private NonVolatile conditionName = NonVolatile.Freeze;
        [SerializeField] private int n;
        [SerializeField] private Chat onEffectChat = null, onContinuousEffectChat = null, abruptEndChat = null, endEffectChat = null;
        #endregion

        #region Getters
        public override string GetConditionName()
        {
            return conditionName.ToString();
        }
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
        #endregion

        #region In
        public override void Reset()
        {
            done = false;
        }

        public override IEnumerator ActivateCondition(ConditionOversight activator)
        {
            n++;

            if ((Random.Range(0.0f, 1.0f) <= 0.2f) && (n != 5))
            {
                Chat toSend = abruptEndChat.GetChat();
                toSend.AddToOverride("<POKEMON_NAME>", affectedPokemon.GetName());
                ChatMaster.instance.Add(toSend);

                activator.RemoveFromCondition(this);
            }
            else if (n < 5)
            {
                Chat toSend = onContinuousEffectChat.GetChat();
                toSend.AddToOverride("<POKEMON_NAME>", affectedPokemon.GetName());
                ChatMaster.instance.Add(toSend);

                activator.SetIsStunned(true);
            }
            else
            {
                Chat toSend = endEffectChat.GetChat();
                toSend.AddToOverride("<POKEMON_NAME>", affectedPokemon.GetName());
                ChatMaster.instance.Add(endEffectChat);

                activator.RemoveFromCondition(this);
            }
            yield return null;

            done = true;
        }
        #endregion
    }
}