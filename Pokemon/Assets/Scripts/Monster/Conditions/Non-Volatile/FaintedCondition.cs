﻿#region SDK

using System.Collections;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Chat;
using UnityEngine; //Custom

#endregion

namespace Mfknudsen.Monster.Conditions
{
    [CreateAssetMenu(fileName = "Condition", menuName = "Condition/Create new Non-Volatile Condition/Fainted", order = 0)]
    public class FaintedCondition : Condition
    {
        #region Values
        [SerializeField] private NonVolatile conditionName = NonVolatile.Fainted;
        [SerializeField] private Chat.Chat onEffectChat = null;
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
            active = false;
            done = false;
        }

        public override IEnumerator ActivateCondition(ConditionOversight activator)
        {
            Chat.Chat toSend = onEffectChat.GetChat();
            toSend.AddToOverride("<POKEMON_NAME>", affectedPokemon.GetName());
            ChatMaster.instance.Add(toSend);

            GameObject obj = affectedPokemon.GetSpawnedObject();

            while (!ChatMaster.instance.GetIsClear() && obj != null)
            {
                if (obj.transform.localScale.y > 0.01f)
                    obj.transform.localScale += -Vector3.one * Time.deltaTime;

                yield return null;
            }

            BattleMaster.instance.DespawnPokemon(affectedPokemon);

            yield return new WaitForSeconds(1);

            done = true;
        }
        #endregion
    }
}