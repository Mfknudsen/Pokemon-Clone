#region SDK

using System.Collections;
using Mfknudsen.Comunication;
using Mfknudsen.Pokémon;
using Mfknudsen.Pokémon.Conditions;
using UnityEngine; //Custom

#endregion

namespace Mfknudsen.Items.Medicine
{
    [CreateAssetMenu(fileName = "Item", menuName = "Item/Create new Revive")]
    public class Revive : Item, IBattleItem
    {
        #region Values

        [SerializeField] private bool toFull;
        [SerializeField] private Chat onActivation;

        #endregion

        #region Override

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

        public override IEnumerator Activate()
        {
            done = false;

            Chat toSend = onActivation.GetChat();
            toSend.AddToOverride("<POKEMON_NAME>", target.GetName());
            ChatMaster.instance.Add(toSend);

            target.GetConditionOversight().TryApplyNonVolatileCondition(null);

            if (toFull)
                target.ReceiveDamage(-Mathf.Infinity);
            else
                target.ReceiveDamage(-(target.GetStat(Stat.HP) / 2));

            while (!ChatMaster.instance.GetIsClear())
                yield return null;

            done = true;
        }

        #endregion
    }
}