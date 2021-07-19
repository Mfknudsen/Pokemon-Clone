#region SDK

using System.Collections;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Comunication;
using Mfknudsen.Pokémon;
using Mfknudsen.Pokémon.Conditions;
using UnityEngine; //Custom

#endregion

namespace Mfknudsen.Items.Medicine
{
    [CreateAssetMenu(fileName = "Item", menuName = "Item/Create new Potion")]
    public class Potion : Item, IBattleItem
    {
        #region Values

        [SerializeField] private float healAmount;
        [SerializeField] private Chat onActivation;

        #endregion

        #region Overrides

        public override bool IsUsableTarget(Pokemon p)
        {
            if (p.GetConditionOversight().GetNonVolatileStatus() != null)
            {
                if (!(p.GetConditionOversight().GetNonVolatileStatus() is FaintedCondition))
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
            float curHeal = 0, healSpeed = healAmount / 200 * BattleMaster.instance.GetSecPerPokeMove();

            while (curHeal < healAmount)
            {
                if (curHeal + healSpeed < healAmount)
                {
                    target.ReceiveDamage(-healSpeed);
                    curHeal += healSpeed;
                    yield return new WaitForSeconds(BattleMaster.instance.GetSecPerPokeMove() /
                                                    (200 * BattleMaster.instance.GetSecPerPokeMove()));
                }
                else
                {
                    target.ReceiveDamage(-(healAmount - curHeal));
                    curHeal += healSpeed;
                    yield return null;
                }
            }

            Chat toSend = onActivation.GetChat();
            toSend.AddToOverride("<POKEMON_NAME>", target.GetName());
            ChatMaster.instance.Add(toSend);

            while (!ChatMaster.instance.GetIsClear())
                yield return null;

            done = true;
        }

        #endregion
    }
}