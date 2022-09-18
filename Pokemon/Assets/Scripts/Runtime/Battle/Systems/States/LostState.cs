#region SDK

using System.Collections;
using Runtime.Communication;
using Runtime.Player;
using Runtime.Systems.Operation;
using Runtime.Systems.UI;

#endregion

namespace Runtime.Battle.Systems.States
{
    public class LostState : State
    {
        public LostState(BattleManager battleManager, OperationManager operationManager, ChatManager chatManager,
            UIManager uiManager, PlayerManager playerManager) : base(battleManager, operationManager, chatManager,
            uiManager, playerManager)
        {
        }

        public override IEnumerator Tick()
        {
            this.battleManager.EndBattle(false);

            yield break;
        }
    }
}