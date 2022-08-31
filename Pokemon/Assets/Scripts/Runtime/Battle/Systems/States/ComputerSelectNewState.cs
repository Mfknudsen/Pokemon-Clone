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

        public ComputerSelectNewState(BattleManager manager, List<SwitchAction> switchActions) : base(manager)
        {
            this.switchActions = switchActions;
        }

        public override IEnumerator Tick()
        {
            BattleMember playerBattleMember = PlayerManager.instance.GetBattleMember();

            foreach (Spot spot in this.manager.GetSpotOversight().GetSpots())
            {
                BattleMember battleMember = spot.GetBattleMember();

                if (spot.GetActivePokemon() == null ||
                      battleMember == playerBattleMember ||
                      !battleMember.GetTeam().CanSendMorePokemon()) continue;

                battleMember.ActivateAIBrain(spot.GetActivePokemon());
            }

            this.manager.SetState(new SwitchNewInState(this.manager, this.switchActions));

            yield break;
        }
    }
}