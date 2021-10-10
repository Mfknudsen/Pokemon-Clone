#region Packages

using System.Collections;
using Mfknudsen.Communication;
using Mfknudsen.Pokémon;
using Mfknudsen.Pokémon.Conditions.Non_Volatiles;
using UnityEngine;

#endregion

namespace Mfknudsen.Items.Medicine
{
    [CreateAssetMenu(fileName = "Item", menuName = "Item/Create new Revive")]
    public class Revive : BattleItem
    {
        #region Values

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
            ChatManager.instance.Add(toSend);

            target.GetConditionOversight().TryApplyNonVolatileCondition(null);

            if (toFull)
                target.ReceiveDamage(-Mathf.Infinity);
            else
                target.ReceiveDamage(-(target.GetStat(Stat.HP) / 2));

            while (!ChatManager.instance.GetIsClear())
                yield return null;

            done = true;
        }

        #endregion
    }
}