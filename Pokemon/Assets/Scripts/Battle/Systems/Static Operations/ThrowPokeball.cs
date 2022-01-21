#region Packages

using System.Collections;
using Mfknudsen.Communication;
using Mfknudsen.Player;
using Mfknudsen.Pokémon;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.Systems.Static_Operations
{
    public class ThrowPokeball : IOperation
    {
        #region Values

        private bool done;
        private readonly Pokemon target;
        private readonly int clicks;
        private readonly Chat resultChat;

        #endregion

        public ThrowPokeball(Pokemon target, int clicks, Chat resultChat)
        {
            this.target = target;
            this.clicks = clicks;
            this.resultChat = resultChat;
        }

        #region IOperation

        public bool Done()
        {
            return done;
        }

        public IEnumerator Operation()
        {
            done = false;
            OperationManager operationManager = OperationManager.Instance;
            OperationsContainer container = new OperationsContainer();

            #region Trap Target

            Transform targetTransform = target.GetSpawnedObject().transform;
            while (targetTransform.localScale.x > 0.1f)
            {
                targetTransform.localScale -= Vector3.one * Time.deltaTime;
                yield return null;
            }

            #endregion

            #region Clicks

            for (int i = 0; i < 3; i++)
            {
                Debug.Log("CLICK");

                yield return new WaitForSeconds(1.5f);

                if (i == clicks)
                    break;
            }

            #endregion

            #region Final

            ChatOperation chatOperation = new ChatOperation(resultChat);
            container.Add(chatOperation);
            operationManager.AddOperationsContainer(container);

            if (clicks == 4)
            {
                Debug.Log("TICK");

                BattleManager.instance.DespawnPokemon(target);

                container = new OperationsContainer();
                container.Add(new CaughtPokemon(target, PlayerManager.Instance.GetTeam()));
                operationManager.AddOperationsContainer(container);
            }
            else
            {
                while (targetTransform.localScale.x < 1)
                {
                    targetTransform.localScale += Vector3.one * (3.5f * Time.deltaTime);

                    yield return null;
                }

                targetTransform.localScale = Vector3.one;
            }

            #endregion

            yield return new WaitForSeconds(1);

            done = true;
        }

        public void End()
        {
        }

        #endregion
    }
}