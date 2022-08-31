#region Packages

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Runtime.Battle.Actions;
using Runtime.Battle.Systems.Spots;
using Runtime.Battle.UI.Selection;
using Runtime.Communication;
using Runtime.Player;
using UnityEngine;

#endregion

namespace Runtime.Battle.Systems.States
{
    public class PlayerSelectNewState : State
    {
        public PlayerSelectNewState(BattleManager manager) : base(manager)
        {
        }

        public override IEnumerator Tick()
        {
            Cursor.visible = true;
            List<SwitchAction> switchActions = new();
            SpotOversight oversight = this.manager.GetSpotOversight();
            BattleMember playerTeam = PlayerManager.instance.GetBattleMember();

            if (playerTeam.GetTeam().CanSendMorePokemon())
            {
                foreach (Spot spot in oversight.GetSpots()
                             .Where(s =>
                                 s.GetBattleMember() == playerTeam ||
                                 s.GetActivePokemon() != null))
                {
                    SwitchAction switchAction = this.manager.InstantiateSwitchAction();

                    switchAction.SetSpot(spot);

                    this.manager.GetSelectionMenu().DisplaySelection(SelectorGoal.Switch, switchAction);

                    yield return new WaitWhile(() => !switchAction.GetNextPokemon() ||
                                                     !ChatManager.instance.GetIsClear());

                    switchActions.Add(switchAction);
                }
            }

            this.manager.GetSelectionMenu().DisableDisplaySelection();

            Cursor.visible = false;

            this.manager.SetState(new ComputerSelectNewState(this.manager, switchActions));
        }
    }
}