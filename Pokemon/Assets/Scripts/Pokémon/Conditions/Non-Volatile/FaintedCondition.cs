#region SDK

using System.Collections;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Comunication;
using UnityEngine;

//Custom

#endregion

namespace Mfknudsen.Pokémon.Conditions
{
    [CreateAssetMenu(fileName = "Condition", menuName = "Condition/Create new Non-Volatile Condition/Fainted",
        order = 0)]
    public class FaintedCondition : Condition, INonVolatile
    {
        #region Values

        [SerializeField] private Chat onEffectChat;

        #endregion

        #region Getters

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