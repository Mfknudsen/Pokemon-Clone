using System.Collections;
using Runtime.Communication;
using Runtime.Player;
using Runtime.Systems.Operation;
using Runtime.Systems.UI;

namespace Runtime.Battle.Systems.States
{
    public abstract class State
    {
        protected readonly BattleManager battleManager;
        protected readonly OperationManager operationManager;
        protected readonly ChatManager chatManager;
        protected readonly UIManager uiManager;
        protected readonly PlayerManager playerManager;

        protected State(BattleManager battleManager, OperationManager operationManager, ChatManager chatManager, UIManager uiManager, PlayerManager playerManager)
        {
            this.battleManager = battleManager;
            this.operationManager = operationManager;
            this.chatManager = chatManager;
            this.uiManager = uiManager;
            this.playerManager = playerManager;
        }

        public abstract IEnumerator Tick();
    }
}