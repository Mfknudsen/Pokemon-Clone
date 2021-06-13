#region SDK

using System.Collections;
using Mfknudsen.Chat;
using Mfknudsen.Monster;
using Mfknudsen.Monster.Conditions;
using UnityEngine; //Custom

#endregion

namespace Mfknudsen.Items.Medicine
{
    [CreateAssetMenu(fileName = "Item", menuName = "Item/Create new Revive")]
    public class Revive : Item
    {
        #region Values
        [Header("Revive:")]
        [SerializeField] private bool toFull = false;
        [SerializeField] private Chat.Chat onActivation = null;
        #endregion

        #region Override
        public override void SetTarget(Pokemon set)
        {
            base.SetTarget(set);

            set.SetRevived(true);
        }

        public override bool IsUsableTarget(Pokemon p)
        {
            if (p.GetConditionOversight().GetNonVolatileStatus() != null)
            {
                if (p.GetConditionOversight().GetNonVolatileStatus().GetConditionName() == NonVolatile.Fainted.ToString() && !p.GetRevived())
                    return true;
            }

            return false;
        }

        public override IEnumerator Activate()
        {
            done = false;

            Chat.Chat toSend = onActivation.GetChat();
            toSend.AddToOverride("<POKEMON_NAME>", target.GetName());
            ChatMaster.instance.Add(toSend);

            target.GetConditionOversight().TryApplyNonVolatileCondition(null);
            if (toFull)
                target.RecieveDamage(-Mathf.Infinity);
            else
                target.RecieveDamage(-(target.GetStat(Stat.HP) / 2));

            while (!ChatMaster.instance.GetIsClear())
                yield return null;

            yield return null;
            done = true;
        }
        #endregion
    }
}