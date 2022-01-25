#region Packages

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mfknudsen.Battle.Actions;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Battle.UI.Selection;
using Mfknudsen.Communication;
using Mfknudsen.Player;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class PlayerSelectNewState : State
    {
        public PlayerSelectNewState(BattleManager manager) : base(manager)
        {
        }

        public override IEnumerator Tick()
        {
            List<SwitchAction> switchActions = new List<SwitchAction>();
            SpotOversight oversight = manager.GetSpotOversight();
            BattleMember playerTeam = PlayerManager.instance.GetBattleMember();

            if (playerTeam.GetTeam().CanSendMorePokemon())
            {
                foreach (Spot spot in oversight.GetSpots()
                    .Where(s =>
                        s.GetBattleMember() == playerTeam ||
                        s.GetActivePokemon() != null))
                {
                    SwitchAction switchAction = manager.InstantiateSwitchAction();

                    switchAction.SetSpot(spot);

                    manager.GetSelectionMenu().DisplaySelection(SelectorGoal.Switch, switchAction);

                    while (switchAction.GetNextPokemon() is null || !ChatManager.instance.GetIsClear())
                        yield return null;

                    switchActions.Add(switchAction);
                }
            }

            manager.GetSelectionMenu().DisableDisplaySelection();

            manager.SetState(new ComputerSelectNewState(manager, switchActions));
        }
    }
}