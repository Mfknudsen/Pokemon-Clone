#region Packages

using Mfknudsen.Battle.Systems;
using Mfknudsen.Battle.Systems.Spots;

#endregion

namespace Mfknudsen.AI.Virtual
{
    public class VirtualBattle
    {
        public readonly VirtualSpotOversight spotOversight;

        public VirtualBattle()
        {
            spotOversight = new VirtualSpotOversight();

            foreach (Spot spot in BattleManager.instance.GetSpotOversight().GetSpots())
                spotOversight.spots.Add(new VirtualSpot(spot.GetActivePokemon()));
        }

        public VirtualBattle(VirtualBattle oldBattle)
        {
            spotOversight = new VirtualSpotOversight();
            
            foreach (VirtualSpot spot in oldBattle.spotOversight.spots)
                spotOversight.spots.Add(new VirtualSpot(spot.virtualPokemon));
        }
    }
}