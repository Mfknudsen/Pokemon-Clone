#region SDK

using System.Collections;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Battle.UI.Selection;
using Mfknudsen.Pokémon;
using Mfknudsen.Trainer;
using Mfknudsen.Player;
using Mfknudsen.UI.Cursor;

#endregion

namespace Mfknudsen.Battle.Systems.States
{
    public class PlayerTurnState : State
    {
        public PlayerTurnState(BattleManager manager) : base(manager)
        {
        }

        public override IEnumerator Tick()
        {
            CustomCursor.ShowCursor();
            Team playerTeam = PlayerManager.instance.GetTeam();
            SpotOversight spotOversight = manager.GetSpotOversight();

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (Spot spot in spotOversight.GetSpots())
            {
                Pokemon pokemon = spot.GetActivePokemon();

                if (pokemon is null || !playerTeam.PartOfTeam(pokemon)) continue;

                manager.GetSelectionMenu().DisplaySelection(SelectorGoal.Turn, pokemon);

                while (pokemon.GetBattleAction() is null)
                    yield return null;
            }

            manager.GetSelectionMenu().DisableDisplaySelection();
            
            CustomCursor.HideCursor();
            manager.SetState(new ComputerTurnState(manager));
        }
    }
}