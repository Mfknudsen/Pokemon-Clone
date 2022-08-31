#region Packages

using System.Collections;
using System.Linq;
using Runtime.Communication;

#endregion

namespace Runtime.Battle.Systems.States
{
    public class WinState : State
    {
        public WinState(BattleManager manager) : base(manager)
        {
        }

        public override IEnumerator Tick()
        {
            ChatManager chatManager = ChatManager.instance;

            foreach (BattleMember battleMember in this.manager.GetSpotOversight().GetSpots()
                         .Select(s =>
                             s.GetBattleMember())
                         .Where(bm =>
                             !bm.GetTeamAffiliation()))
                chatManager.Add(battleMember.GetOnDefeatedChats());

            yield return null;

            while (!chatManager.GetIsClear())
                yield return null;

            this.manager.EndBattle(true);
        }
    }
}