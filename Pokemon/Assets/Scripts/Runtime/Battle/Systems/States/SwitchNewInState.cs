#region Packages

using System.Collections;
using System.Collections.Generic;
using Runtime.Battle.Actions;
using Runtime.Communication;
using Runtime.Player;
using Runtime.Systems.Operation;
using Runtime.Systems.UI;

#endregion

// ReSharper disable ParameterHidesMember
namespace Runtime.Battle.Systems.States
{
    public class SwitchNewInState : State
    {
        private readonly List<SwitchAction> switchActions;

        public SwitchNewInState(BattleManager battleManager, OperationManager operationManager, ChatManager chatManager,
            UIManager uiManager, PlayerManager playerManager, List<SwitchAction> switchActions) : base(battleManager,
            operationManager, chatManager, uiManager, playerManager)
        {
            this.switchActions = switchActions;
        }

        public override IEnumerator Tick()
        {
            foreach (SwitchAction switchAction in this.switchActions)
            {
                OperationsContainer container = new();
                container.Add(switchAction);
                this.operationManager.AddOperationsContainer(container);

                while (!switchAction.IsOperationDone() || !this.chatManager.GetIsClear())
                    yield return null;
            }

            this.battleManager.SetState(new RoundDoneState(this.battleManager, this.operationManager, this.chatManager, this.uiManager, this.playerManager));
        }
    }
}