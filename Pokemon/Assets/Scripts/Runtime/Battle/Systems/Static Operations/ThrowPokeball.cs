#region Packages

using System.Collections;
using Runtime.Communication;
using Runtime.Player;
using Runtime.Pokémon;
using Runtime.Systems.Operation;
using UnityEngine;

#endregion

namespace Runtime.Battle.Systems.Static_Operations
{
    public class ThrowPokeball : IOperation
    {
        #region Values

        private readonly PlayerManager playerManager;
        private readonly OperationManager operationManager;
        private bool done;
        private readonly Pokemon target;
        private readonly int clicks;
        private readonly Chat resultChat;

        #endregion

        public ThrowPokeball(PlayerManager playerManager, OperationManager operationManager, Pokemon target, int clicks,
            Chat resultChat)
        {
            this.playerManager = playerManager;
            this.operationManager = operationManager;
            this.target = target;
            this.clicks = clicks;
            this.resultChat = resultChat;
        }

        #region IOperation

        public bool IsOperationDone()
        {
            return done;
        }

        public IEnumerator Operation()
        {
            done = false;
            OperationsContainer container = new();

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

            ChatOperation chatOperation = new(resultChat);
            container.Add(chatOperation);
            operationManager.AddOperationsContainer(container);

            if (clicks == 4)
            {
                Debug.Log("TICK");

                BattleManager.instance.DespawnPokemon(target);

                container = new OperationsContainer();
                container.Add(new CaughtPokemon(target, playerManager.GetTeam()));
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

        public void OperationEnd()
        {
        }

        #endregion
    }
}