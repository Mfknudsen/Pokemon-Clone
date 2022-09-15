#region Packages

using System.Collections;
using System.Collections.Generic;
using Runtime.Battle.Actions;
using Runtime.Battle.Systems.Spots;
using Runtime.Player;

#endregion

namespace Runtime.Battle.Systems.States
{
    public class ComputerSelectNewState : State
    {
        private readonly List<SwitchAction> switchActions;

        public ComputerSelectNewState(BattleManager battleManager, List<SwitchAction> switchActions) : base(battleManager)
        {
            this.switchActions = switchActions;
        }

        public override IEnumerator Tick()
        {
            BattleMember playerBattleMember = PlayerManager.instance.GetBattleMember();

            foreach (Spot spot in this.battleManager.GetSpotOversight().GetSpots())
            {
                BattleMember battleMember = spot.GetBattleMember();

                if (spot.GetActivePokemon() == null ||
                      battleMember == playerBattleMember ||
                      !battleMember.GetTeam().CanSendMorePokemon()) continue;

                battleMember.ActivateAIBrain(spot.GetActivePokemon());
            }

            this.battleManager.SetState(new SwitchNewInState(this.battleManager, this.switchActions));

            yield break;
        }
    }
}