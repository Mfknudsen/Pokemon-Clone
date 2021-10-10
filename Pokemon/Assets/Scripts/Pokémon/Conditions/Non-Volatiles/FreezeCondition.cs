#region Packages

using System.Collections;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Communication;
using UnityEngine;

#endregion

namespace Mfknudsen.Pokémon.Conditions.Non_Volatiles
{
    [CreateAssetMenu(fileName = "Condition", menuName = "Condition/Create new Non-Volatile Condition/Freeze",
        order = 1)]
    public class FreezeCondition : NonVolatileCondition, IOperation
    {
        #region Values

        [SerializeField] private int n;

        [SerializeField] private Chat onEffectChat,
            onContinuousEffectChat,
            abruptEndChat,
            endEffectChat;

        private bool done;

        #endregion

        #region Getters

        public override Condition GetCondition()
        {
            Condition result = this;

            if (result.GetIsInstantiated()) return result;

            result = Instantiate(this);
            result.SetIsInstantiated(true);

            return result;
        }

        #endregion

        #region In

        public override void Reset()
        {
            done = false;
        }

        public IEnumerator Operation()
        {
            n++;

            if ((Random.Range(0.0f, 1.0f) <= 0.2f) && (n != 5))
            {
                Chat toSend = abruptEndChat.GetChat();
                toSend.AddToOverride("<POKEMON_NAME>", affectedPokemon.GetName());
                ChatManager.instance.Add(toSend);

                conditionOversight.RemoveFromCondition(this);
            }
            else if (n < 5)
            {
                Chat toSend = onContinuousEffectChat.GetChat();
                toSend.AddToOverride("<POKEMON_NAME>", affectedPokemon.GetName());
                ChatManager.instance.Add(toSend);

                conditionOversight.SetIsStunned(true);
            }
            else
            {
                Chat toSend = endEffectChat.GetChat();
                toSend.AddToOverride("<POKEMON_NAME>", affectedPokemon.GetName());
                ChatManager.instance.Add(endEffectChat);

                conditionOversight.RemoveFromCondition(this);
            }

            yield return null;

            done = true;
        }

        public void End()
        {
        }

        #endregion

        public bool Done()
        {
            return done;
        }
    }
}