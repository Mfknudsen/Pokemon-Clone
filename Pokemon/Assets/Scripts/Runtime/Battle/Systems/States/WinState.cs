#region Packages

using System.Collections;
using System.Linq;
using Runtime.Communication;

#endregion

namespace Runtime.Battle.Systems.States
{
    public class WinState : State
    {
        public WinState(BattleManager battleManager) : base(battleManager)
        {
        }

        public override IEnumerator Tick()
        {
            ChatManager chatManager = ChatManager.instance;

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