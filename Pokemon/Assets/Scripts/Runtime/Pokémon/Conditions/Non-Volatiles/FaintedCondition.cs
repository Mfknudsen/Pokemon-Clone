#region Packages

using System.Collections;
using Runtime.Battle.Systems;
using Runtime.Communication;
using Runtime.Systems;
using UnityEngine;

#endregion

namespace Runtime.Pokémon.Conditions.Non_Volatiles
{
    [CreateAssetMenu(fileName = "Condition", menuName = "Condition/Create new Non-Volatile Condition/Fainted",
        order = 0)]
    public class FaintedCondition : NonVolatileCondition, IOperation
    {
        #region Values

        [SerializeField] private Chat onEffectChat;
        private bool done;

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

        public bool Done()
        {
            return done;
        }

        public IEnumerator Operation ()
        {
            Chat toSend = onEffectChat.GetChat();
            toSend.AddToOverride("<POKEMON_NAME>", affectedPokemon.GetName());
            ChatManager.instance.Add(toSend);

            GameObject obj = affectedPokemon.GetSpawnedObject();

            while (!ChatManager.instance.GetIsClear())
            {
                if (obj.transform.localScale.y > 0.01f)
                    obj.transform.localScale += -Vector3.one * Time.deltaTime;

                yield return null;
            }

            BattleManager.instance.DespawnPokemon(affectedPokemon);

            yield return new WaitForSeconds(1);

            done = true;
        }

        public void End()
        {
        }

        #endregion
    }
}