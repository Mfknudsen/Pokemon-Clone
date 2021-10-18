#region Packages

using System.Collections.Generic;
using System.Linq;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Weathers;
using UnityEngine;

#endregion

namespace Mfknudsen.AI.Virtual
{
    public class VirtualBattle
    {
        public readonly VirtualSpotOversight spotOversight;
        public readonly List<Weather> weathers;

        public VirtualBattle()
        {
            spotOversight = new VirtualSpotOversight();
            BattleManager battleManager = BattleManager.instance;

            foreach (Spot spot in battleManager.GetSpotOversight().GetSpots())
                spotOversight.spots.Add(new VirtualSpot(spot.GetActivePokemon()));

            weathers = new List<Weather>();
            weathers.AddRange(battleManager.GetWeatherManager().GetAll().Select(Object.Instantiate));
        }

        public VirtualBattle(VirtualBattle oldBattle)
        {
            spotOversight = new VirtualSpotOversight();

            foreach (VirtualSpot spot in oldBattle.spotOversight.spots)
                spotOversight.spots.Add(new VirtualSpot(spot.virtualPokemon));

            weathers = new List<Weather>();
            foreach (Weather w in oldBattle.weathers.Select(Object.Instantiate))
            {
                w.TickTurn();
                
                if (w.EffectDone())
                    continue;
                
                weathers.Add(w);
            }
        }
    }
}