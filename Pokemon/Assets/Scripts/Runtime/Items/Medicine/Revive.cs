﻿#region Packages

using System.Collections;
using Runtime.Communication;
using Runtime.Pokémon;
using Runtime.Pokémon.Conditions.Non_Volatiles;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Runtime.Items.Medicine
{
    [CreateAssetMenu(fileName = "Item", menuName = "Item/Create new Revive")]
    public class Revive : BattleItem
    {
        #region Values

        [SerializeField, Required] private ChatManager chatManager;
        [SerializeField] private bool toFull;
        [SerializeField] private Chat onActivation;

        #endregion

        #region Overrides

        public override void SetTarget(Pokemon set)
        {
            base.SetTarget(set);

            set.SetRevived(true);
        }

        public override bool IsUsableTarget(Pokemon p)
        {
            if (p.GetConditionOversight().GetNonVolatileStatus() is null) return false;

            return p.GetConditionOversight().GetNonVolatileStatus() is FaintedCondition && !p.GetRevived();
        }

        public override IEnumerator Operation()
        {
            done = false;

            Chat toSend = onActivation.GetChat();
            toSend.AddToOverride("<POKEMON_NAME>", target.GetName());
            chatManager.Add(toSend);

            target.GetConditionOversight().TryApplyNonVolatileCondition(null);

            if (toFull)
                target.ReceiveDamage(-Mathf.Infinity);
            else
                target.ReceiveDamage(-(target.GetCalculatedStat(Stat.HP) / 2));

            while (!chatManager.GetIsClear())
                yield return null;

            done = true;
        }

        #endregion
    }
}