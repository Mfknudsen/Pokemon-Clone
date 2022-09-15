using System.Collections;
using Runtime.Communication;
using Runtime.Systems;

namespace Runtime.Battle.Systems.States
{
    public abstract class State
    {
        protected readonly BattleManager battleManager;
        protected readonly OperationManager operationManager;
        protected readonly ChatManager chatManager;

        protected State(BattleManager battleManager, OperationManager operationManager, ChatManager chatManager)
        {
            this.battleManager = battleManager;
            this.operationManager = operationManager;
            this.chatManager = chatManager;
        }

        public abstract IEnumerator Tick();
    }
}