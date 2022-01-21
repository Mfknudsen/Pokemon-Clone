#region Packages

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Player;

#endregion

namespace Mfknudsen.Battle.Systems.States
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
            BattleMember playerBattleMember = PlayerManager.Instance.GetBattleMember();

            foreach (Spot spot in manager.GetSpotOversight().GetSpots())
            {
                BattleMember battleMember = spot.GetBattleMember();

                if (spot.GetActivePokemon() == null ||
                      battleMember == playerBattleMember ||
                      !battleMember.GetTeam().CanSendMorePokemon()) continue;

                #region Send Information
                

                #endregion

                battleMember.ActivateAIBrain(spot.GetActivePokemon());
            }

            manager.SetState(new SwitchNewInState(manager, switchActions));

            yield break;
        }
    }
}