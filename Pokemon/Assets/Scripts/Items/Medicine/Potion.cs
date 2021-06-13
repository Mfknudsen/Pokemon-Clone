﻿#region SDK

using System.Collections;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Chat;
using Mfknudsen.Monster;
using Mfknudsen.Monster.Conditions;
using UnityEngine; //Custom

#endregion

namespace Mfknudsen.Items.Medicine
{
    [CreateAssetMenu(fileName = "Item", menuName = "Item/Create new Potion")]
    public class Potion : Item
    {
        #region Values
        [Header("Potion:")]
        [SerializeField] private float healAmount = 0;
        [SerializeField] private Chat.Chat onActivation = null;
        #endregion

        #region Overrides
        public override bool IsUsableTarget(Pokemon p)
        {
            if (p.GetConditionOversight().GetNonVolatileStatus() != null)
            {
                if (p.GetConditionOversight().GetNonVolatileStatus().GetConditionName() != NonVolatile.Fainted.ToString())
                    return true;
            }
            else
                return true;
            Debug.Log("Error");
            return false;
        }

        public override IEnumerator Activate()
        {
            done = false;
            float curheal = 0, healSpeed = healAmount / 200 * BattleMaster.instance.GetSecPerPokeMove();

            while (curheal < healAmount)
            {
                if (curheal + healSpeed < healAmount)
                {
                    target.RecieveDamage(-healSpeed);
                    curheal += healSpeed;
                    yield return new WaitForSeconds(BattleMaster.instance.GetSecPerPokeMove() / (200 * BattleMaster.instance.GetSecPerPokeMove()));
                }
                else
                {
                    target.RecieveDamage(-(healAmount - curheal));
                    curheal += healSpeed;
                    yield return null;
                }
            }

            Chat.Chat toSend = onActivation.GetChat();
            toSend.AddToOverride("<POKEMON_NAME>", target.GetName());
            ChatMaster.instance.Add(toSend);

            while (!ChatMaster.instance.GetIsClear())
                yield return null;

            done = true;
        }
        #endregion
    }
}