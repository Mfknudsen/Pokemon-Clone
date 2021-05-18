#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Custom
using Battle;
using Communications;
#endregion

namespace Monster.Conditions.Non_Volatile
{
    [CreateAssetMenu(fileName = "Condition", menuName = "Condition/Create new Non-Volatile Condition/Fainted", order = 0)]
    public class FaintedCondition : Condition
    {
        #region Values
        [SerializeField] private NonVolatile conditionName = NonVolatile.Fainted;
        [SerializeField] private Chat onEffectChat = null;
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
            Chat toSend = onEffectChat.GetChat();
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