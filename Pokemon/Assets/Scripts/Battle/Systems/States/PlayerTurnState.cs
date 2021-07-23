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
        public PlayerTurnState(BattleMaster master) : base(master)
        {
        }

        public override IEnumerator Tick()
        {
            Team playerTeam = MasterPlayer.instance.GetTeam();
            SpotOversight spotOversight = master.GetSpotOversight();

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (Spot spot in spotOversight.GetSpots())
            {
                Pokemon pokemon = spot.GetActivePokemon();

                if (pokemon is null || !playerTeam.PartOfTeam(pokemon)) continue;

                master.GetSelectionMenu().DisplaySelection(SelectorGoal.Turn, pokemon);

                while (pokemon.GetBattleAction() is null)
                    yield return null;
            }

            master.GetSelectionMenu().DisableDisplaySelection();
            
            master.SetState(new ComputerTurnState(master));
        }
    }
}