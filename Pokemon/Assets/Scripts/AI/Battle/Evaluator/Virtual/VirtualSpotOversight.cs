#region Packages

using System.Collections.Generic;
using System.Linq;
using Mfknudsen.Battle.Systems;
using Mfknudsen.Battle.Systems.Spots;
using Mfknudsen.Pokémon;
using UnityEngine;

#endregion

namespace Mfknudsen.AI.Virtual
{
    public class VirtualSpotOversight
    {
        public readonly List<VirtualSpot> spots;

        public VirtualSpotOversight()
        {
            spots = new List<VirtualSpot>();

            foreach (Spot spot in BattleManager.instance.GetSpotOversight().GetSpots())
            {
                VirtualSpot virtualSpot = new VirtualSpot(spot.GetActivePokemon());
                virtualSpot.SetRelations(
                    spot.GetLeft()?.GetActivePokemon(),
                    spot.GetRight()?.GetActivePokemon(),
                    spot.GetStrafeLeft()?.GetActivePokemon(),
                    spot.GetStrafeRight()?.GetActivePokemon(),
                    spot.GetFront()?.GetActivePokemon()
                );
                spots.Add(virtualSpot);
            }
        }

        public VirtualSpot GetPokemonSpot(Pokemon pokemon)
        {
            foreach (VirtualSpot virtualSpot in spots)
            {
                VirtualPokemon virtualPokemon = virtualSpot.virtualPokemon;
                if (virtualPokemon.GetActualPokemon() == pokemon || virtualPokemon.GetFakePokemon() == pokemon)
                    return virtualSpot;
            }

            return null;
        }
    }

    public class VirtualSpot
    {
        public readonly VirtualPokemon virtualPokemon;
        public Pokemon left, right, strafeRight, strafeLeft, front;

        public VirtualSpot(Pokemon pokemon)
        {
            virtualPokemon = new VirtualPokemon(pokemon);
        }

        public VirtualSpot(VirtualPokemon virtualPokemon)
        {
            this.virtualPokemon = new VirtualPokemon(virtualPokemon.GetFakePokemon());
        }

        public void SetRelations(Pokemon l, Pokemon r, Pokemon sR, Pokemon sL, Pokemon f)
        {
            left = l;
            right = r;
            strafeLeft = sL;
            strafeRight = sR;
            front = f;
        }
    }
}