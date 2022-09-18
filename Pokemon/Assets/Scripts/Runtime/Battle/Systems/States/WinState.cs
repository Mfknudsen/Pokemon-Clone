#region Packages

using System.Collections;
using System.Linq;
using Runtime.Communication;
using Runtime.Player;
using Runtime.Systems.Operation;
using Runtime.Systems.UI;

#endregion

namespace Runtime.Battle.Systems.States
{
    public class WinState : State
    {
        public WinState(BattleManager battleManager, OperationManager operationManager, ChatManager chatManager, UIManager uiManager, PlayerManager playerManager) : base(battleManager, operationManager, chatManager, uiManager, playerManager)
        {
        }

        public override IEnumerator Tick()
        {
            foreach (BattleMember battleMember in this.battleManager.GetSpotOversight().GetSpots()
                         .Select(s =>
                             s.GetBattleMember())
                         .Where(bm =>
                             !bm.GetTeamAffiliation()))
                chatManager.Add(battleMember.GetOnDefeatedChats());

            yield return null;

            while (!chatManager.GetIsClear())
                yield return null;

            this.battleManager.EndBattle(true);
        }
    }
}