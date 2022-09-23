#region Packages

using System.Collections;
using System.Linq;
using Runtime.Battle.Systems.Spots;
using Runtime.Communication;
using Runtime.Player;
using Runtime.Pokémon.Conditions;
using Runtime.Systems.Operation;
using Runtime.Systems.UI;

#endregion

namespace Runtime.Battle.Systems.States
{
    public class AfterConditionState : State
    {
        private readonly SpotOversight oversight;

        public AfterConditionState(BattleManager battleManager, OperationManager operationManager,
            ChatManager chatManager, UIManager uiManager, PlayerManager playerManager) : base(battleManager,
            operationManager, chatManager, uiManager, playerManager)
        {
            this.oversight = battleManager.GetSpotOversight();
        }

        public override IEnumerator Tick()
        {
            foreach (ConditionOversight conditionOversight in this.oversight.GetSpots()
                         .Where(s => s.GetActivePokemon() != null)
                         .Select(s =>
                             s.GetActivePokemon().GetConditionOversight()))
            {
                this.battleManager.StartCoroutine(conditionOversight.CheckConditionEndTurn());

                while (!conditionOversight.GetDone() || !this.chatManager.GetIsClear())
                    yield return null;

                conditionOversight.Reset();
            }

            if (this.oversight.GetSpots().FirstOrDefault(spot =>
                    spot.GetActivePokemon() == null &&
                    spot.GetBattleMember().GetTeam().CanSendMorePokemon()))
            {
                this.battleManager.SetState(new PlayerSelectNewState(this.battleManager, this.operationManager, this.chatManager, this.uiManager, this.playerManager));
                yield break;
            }

            this.battleManager.SetState(new RoundDoneState(this.battleManager, this.operationManager, this.chatManager, this.uiManager, this.playerManager));
        }
    }
}