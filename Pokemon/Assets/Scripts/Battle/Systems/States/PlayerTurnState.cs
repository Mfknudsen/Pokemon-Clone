#region SDK

using System.Collections;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Battle.UI.Selection;
using Mfknudsen.Pokémon;
using Mfknudsen.Trainer;
using Mfknudsen.Player;

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
            Team playerTeam = PlayerManager.Instance.GetTeam();
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
            
            manager.SetState(new ComputerTurnState(manager));
        }
    }
}