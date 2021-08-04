#region SDK

using System.Collections;
using System.Collections.Generic;
using Mfknudsen.Battle.Actions.Switch;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Battle.UI.Selection;
using Mfknudsen.Comunication;
using Mfknudsen.Player;
using UnityEngine;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class PlayerSelectNewState : State
    {
        public PlayerSelectNewState(BattleMaster master) : base(master)
        {
        }

        public override IEnumerator Tick()
        {
            List<SwitchAction> switchActions = new List<SwitchAction>();
            SpotOversight oversight = master.GetSpotOversight();
            BattleMember playerTeam = PlayerManager.instance.GetBattleMember();

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (Spot spot in oversight.GetSpots())
            {
                if (spot.GetBattleMember() != playerTeam || !playerTeam.GetTeam().CanSendMorePokemon() ||
                    !(spot.GetActivePokemon() is null)) continue;

                SwitchAction switchAction = master.InstantiateSwitchAction();

                switchAction.SetSpot(spot);

                master.GetSelectionMenu().DisplaySelection(SelectorGoal.Switch, switchAction);

                while (switchAction.GetNextPokemon() is null || !ChatMaster.instance.GetIsClear())
                    yield return null;

                switchActions.Add(switchAction);
            }
            
            master.GetSelectionMenu().DisableDisplaySelection();

            master.SetState(new ComputerSelectNewState(master, switchActions));
        }
    }
}