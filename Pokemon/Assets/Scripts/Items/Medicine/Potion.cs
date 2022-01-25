#region Packages

using System.Collections;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Communication;
using Mfknudsen.Pokémon;
using Mfknudsen.Pokémon.Conditions.Non_Volatiles;
using UnityEngine;

#endregion

namespace Mfknudsen.Items.Medicine
{
    [CreateAssetMenu(fileName = "Item", menuName = "Item/Create new Potion")]
    public class Potion : BattleItem
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

        public override IEnumerator Operation()
        {
            done = false;
            float curHeal = 0, healSpeed = healAmount / 200 * BattleManager.instance.GetSecPerPokeMove();

            while (curHeal < healAmount)
            {
                if (curHeal + healSpeed < healAmount)
                {
                    target.ReceiveDamage(-healSpeed);
                    curHeal += healSpeed;
                    yield return new WaitForSeconds(BattleManager.instance.GetSecPerPokeMove() /
                                                    (200 * BattleManager.instance.GetSecPerPokeMove()));
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
            ChatManager.instance.Add(toSend);

            while (!ChatManager.instance.GetIsClear())
                yield return null;

            done = true;
        }

        #endregion
    }
}