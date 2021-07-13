#region SDK

using System.Collections;
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
            SpotOversight oversight = master.GetSpotOversight();
            BattleMember playerTeam = MasterPlayer.instance.GetBattleMember();

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (Spot spot in oversight.GetSpots())
            {
                if(spot.GetBattleMember() != playerTeam || !playerTeam.GetTeam().CanSendMorePokemon() || !(spot.GetActivePokemon() is null)) continue;
                
                spot.SetNeedNew(true);

                master.DisplayPokemonSelect();

                while (spot.GetNeedNew() || !ChatMaster.instance.GetIsClear())
                    yield return null;
            }
            
            master.GetSelectionMenu().DisableDisplaySelection();

            master.SetState(new ComputerSelectNewState(master));
        }
    }
}